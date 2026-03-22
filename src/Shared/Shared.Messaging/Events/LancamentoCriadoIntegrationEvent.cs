namespace Shared.Messaging.Events;

public sealed record LancamentoCriadoIntegrationEvent(
    Guid EventId,
    Guid LancamentoId,
    int Tipo,
    decimal Valor,
    string Descricao,
    DateTime DataOcorrencia
) : IntegrationEvent(EventId, DateTime.UtcNow);
