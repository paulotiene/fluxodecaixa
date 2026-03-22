using System.Text;
using Shared.Messaging.Configuration;
using Shared.Messaging.Events;
using Shared.Messaging.Interfaces;
using Consolidado.Application.UseCases.ProcessarLancamentoCriado;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consolidado.Infrastructure.BackgroundServices;

public sealed class LancamentoCriadoConsumerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<LancamentoCriadoConsumerBackgroundService> _logger;
    private readonly IMessageSerializer _serializer;
    private IConnection? _connection;
    private IModel? _channel;

    public LancamentoCriadoConsumerBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<LancamentoCriadoConsumerBackgroundService> logger,
        IMessageSerializer serializer)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
        _serializer = serializer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EnsureConsumerStarted();

                _logger.LogInformation(
                    "Consumer do consolidado conectado ao RabbitMQ em {Host}:{Port}, fila {QueueName}.",
                    _options.HostName,
                    _options.Port,
                    _options.QueueName);

                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no consumer do consolidado. Nova tentativa em 5 segundos.");
                CleanupRabbitResources();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private void EnsureConsumerStarted()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
        {
            return;
        }

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        if (!string.IsNullOrWhiteSpace(_options.ExchangeName))
        {
            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Direct, durable: _options.Durable);
        }

        _channel.QueueDeclare(_options.QueueName, _options.Durable, false, false, null);

        if (!string.IsNullOrWhiteSpace(_options.ExchangeName))
        {
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey);
        }

        _channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceivedAsync;

        _channel.BasicConsume(_options.QueueName, autoAck: false, consumer);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation("Mensagem recebida no consolidado. DeliveryTag={DeliveryTag} Payload={Payload}", ea.DeliveryTag, json);

            var message = _serializer.Deserialize<LancamentoCriadoIntegrationEvent>(json);

            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ProcessarLancamentoCriadoHandler>();

            await handler.HandleAsync(message, CancellationToken.None);

            _channel.BasicAck(ea.DeliveryTag, false);
            _logger.LogInformation("Mensagem confirmada no consolidado. EventId={EventId}", message.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consumir/processar mensagem de lançamento criado. DeliveryTag={DeliveryTag}", ea.DeliveryTag);
            _channel.BasicNack(ea.DeliveryTag, false, true);
        }
    }

    public override void Dispose()
    {
        CleanupRabbitResources();
        base.Dispose();
    }

    private void CleanupRabbitResources()
    {
        try
        {
            _channel?.Close();
        }
        catch
        {
        }

        try
        {
            _connection?.Close();
        }
        catch
        {
        }

        _channel?.Dispose();
        _connection?.Dispose();
        _channel = null;
        _connection = null;
    }
}
