using Shared.Messaging.Configuration;
using Shared.Messaging.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Lancamentos.Infrastructure.Messaging;

public sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqOptions _options;

    public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        if (!string.IsNullOrWhiteSpace(_options.ExchangeName))
        {
            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Direct, durable: _options.Durable);
        }

        _channel.QueueDeclare(
            queue: _options.QueueName,
            durable: _options.Durable,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        if (!string.IsNullOrWhiteSpace(_options.ExchangeName))
        {
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey);
        }
    }

    public Task PublishAsync(string payload, CancellationToken cancellationToken = default)
    {
        var body = Encoding.UTF8.GetBytes(payload);
        var props = _channel.CreateBasicProperties();
        props.Persistent = _options.Durable;

        _channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: string.IsNullOrWhiteSpace(_options.ExchangeName) ? _options.QueueName : _options.RoutingKey,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
