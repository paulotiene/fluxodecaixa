using Shared.Messaging.Events;
using Shared.Messaging.Interfaces;
using Lancamentos.Application.Intefaces;
using Lancamentos.Domain.Entities;
using Lancamentos.Domain.Enums;

namespace Lancamentos.Application.UseCases.CriarLancamento;

public sealed class CriarLancamentoHandler
{
    private readonly ILancamentoRepository _lancamentoRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageSerializer _serializer;

    public CriarLancamentoHandler(
        ILancamentoRepository lancamentoRepository,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork,
        IMessageSerializer serializer)
    {
        _lancamentoRepository = lancamentoRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
        _serializer = serializer;
    }

    public async Task<Guid> HandleAsync(CriarLancamentoCommand command, CancellationToken cancellationToken)
    {
        if (command.Tipo is < 1 or > 2)
            throw new ArgumentException("Tipo de lançamento inválido.");

        if (command.Valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(command.Descricao))
            throw new ArgumentException("A descrição é obrigatória.");

        var tipo = (TipoLancamento)command.Tipo;

        var lancamento = new Lancamento(
            tipo,
            command.Valor,
            command.Descricao,
            command.DataOcorrencia);

        await _lancamentoRepository.AddAsync(lancamento, cancellationToken);

        var integrationEvent = new LancamentoCriadoIntegrationEvent(
            Guid.NewGuid(),
            lancamento.Id,
            (int)lancamento.Tipo,
            lancamento.Valor,
            lancamento.Descricao,
            lancamento.DataOcorrencia);

        var payload = _serializer.Serialize(integrationEvent);

        var outboxMessage = OutboxMessage.Create(
            integrationEvent.GetType().FullName ?? integrationEvent.GetType().Name,
            payload);

        await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return lancamento.Id;
    }
}