using Shared.Messaging.Interfaces;
using Lancamentos.Application.Intefaces;
using Lancamentos.Application.Dtos;
using Lancamentos.Application.UseCases.CriarLancamento;
using Lancamentos.Domain.Entities;
using System.Text.Json;
using Xunit;

namespace Lancamentos.UnitTests;

public sealed class CriarLancamentoHandlerTests
{
    [Fact]
    public async Task Deve_criar_lancamento_e_registrar_outbox()
    {
        var lancamentoRepository = new FakeLancamentoRepository();
        var outboxRepository = new FakeOutboxRepository();
        var unitOfWork = new FakeUnitOfWork();
        var messageSerializer = new FakeMessageSerializer();

        var handler = new CriarLancamentoHandler(
            lancamentoRepository,
            outboxRepository,
            unitOfWork,
            messageSerializer);

        var id = await handler.HandleAsync(
            new CriarLancamentoCommand
            {
                Tipo = 1,
                Valor = 100.50m,
                Descricao = "Venda teste",
                DataOcorrencia = new DateTime(2026, 03, 20)
            },
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Single(lancamentoRepository.Items);
        Assert.Single(outboxRepository.Items);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    private sealed class FakeLancamentoRepository : ILancamentoRepository
    {
        public List<Lancamento> Items { get; } = new();

        public Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken)
        {
            Items.Add(lancamento);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<LancamentoListItemDto>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyList<LancamentoListItemDto>)new List<LancamentoListItemDto>());

        public Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
    }

    private sealed class FakeOutboxRepository : IOutboxRepository
    {
        public List<OutboxMessage> Items { get; } = new();

        public Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            Items.Add(message);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken)
            => Task.FromResult((IReadOnlyList<OutboxMessage>)Items.Take(take).ToList());
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeMessageSerializer : IMessageSerializer
    {
        public string Serialize<T>(T value)
            => JsonSerializer.Serialize(value);

        public T Deserialize<T>(string value)
            => JsonSerializer.Deserialize<T>(value)!;
    }
}