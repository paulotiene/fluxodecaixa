using Consolidado.Application.Interfaces;
using Consolidado.Application.Dtos;
using Consolidado.Application.UseCases.ProcessarLancamentoCriado;
using Consolidado.Domain.Entities;
using Xunit;
using Shared.Messaging.Events;

namespace Consolidado.UnitTests;

public sealed class ProcessarLancamentoCriadoHandlerTests
{
    [Fact]
    public async Task Deve_processar_mensagem_sem_duplicar_evento()
    {
        var consolidadoRepository = new FakeConsolidadoRepository();
        var processedRepository = new FakeProcessedMessageRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ProcessarLancamentoCriadoHandler(
            consolidadoRepository,
            processedRepository,
            unitOfWork);

        var message = new LancamentoCriadoIntegrationEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            200m,
            "Venda",
            new DateTime(2026, 03, 20));

        await handler.HandleAsync(message, CancellationToken.None);
        await handler.HandleAsync(message, CancellationToken.None);

        var consolidado = await consolidadoRepository.GetByDateAsync(message.DataOcorrencia.Date, CancellationToken.None);

        Assert.NotNull(consolidado);
        Assert.Equal(200m, consolidado!.TotalCreditos);
        Assert.Equal(0m, consolidado.TotalDebitos);
        Assert.Equal(200m, consolidado.Saldo);
        Assert.Single(processedRepository.Items);
    }

    private sealed class FakeConsolidadoRepository : IConsolidadoRepository
    {
        private readonly List<ConsolidadoDiario> _items = new();

        public Task AddAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken)
        {
            _items.Add(consolidado);
            return Task.CompletedTask;
        }

        public Task<ConsolidadoDiario?> GetByDateAsync(DateTime data, CancellationToken cancellationToken)
            => Task.FromResult(_items.FirstOrDefault(x => x.Data == data.Date));

        public Task<ConsolidadoDto?> GetDtoByDateAsync(DateTime data, CancellationToken cancellationToken)
        {
            var item = _items.FirstOrDefault(x => x.Data == data.Date);
            if (item is null) return Task.FromResult<ConsolidadoDto?>(null);
            return Task.FromResult<ConsolidadoDto?>(new ConsolidadoDto(item.Data, item.TotalCreditos, item.TotalDebitos, item.Saldo, item.AtualizadoEmUtc));
        }
    }

    private sealed class FakeProcessedMessageRepository : IProcessedMessageRepository
    {
        public List<ProcessedMessage> Items { get; } = new();

        public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken)
            => Task.FromResult(Items.Any(x => x.EventId == eventId));

        public Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken)
        {
            Items.Add(processedMessage);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(1);
    }
}
