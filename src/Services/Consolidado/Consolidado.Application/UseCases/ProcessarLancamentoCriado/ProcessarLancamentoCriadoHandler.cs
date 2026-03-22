using Shared.Messaging.Events;
using Consolidado.Application.Interfaces;
using Consolidado.Domain.Entities;

namespace Consolidado.Application.UseCases.ProcessarLancamentoCriado;

public sealed class ProcessarLancamentoCriadoHandler
{
    private readonly IConsolidadoRepository _consolidadoRepository;
    private readonly IProcessedMessageRepository _processedMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessarLancamentoCriadoHandler(
        IConsolidadoRepository consolidadoRepository,
        IProcessedMessageRepository processedMessageRepository,
        IUnitOfWork unitOfWork)
    {
        _consolidadoRepository = consolidadoRepository;
        _processedMessageRepository = processedMessageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(LancamentoCriadoIntegrationEvent message, CancellationToken cancellationToken)
    {
        var alreadyProcessed = await _processedMessageRepository.ExistsAsync(message.EventId, cancellationToken);
        if (alreadyProcessed)
        {
            return;
        }

        var consolidado = await _consolidadoRepository.GetByDateAsync(message.DataOcorrencia.Date, cancellationToken);
        if (consolidado is null)
        {
            consolidado = new ConsolidadoDiario(message.DataOcorrencia.Date);
            await _consolidadoRepository.AddAsync(consolidado, cancellationToken);
        }

        consolidado.AplicarLancamento(message.Tipo, message.Valor);

        await _processedMessageRepository.AddAsync(new ProcessedMessage(message.EventId), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
