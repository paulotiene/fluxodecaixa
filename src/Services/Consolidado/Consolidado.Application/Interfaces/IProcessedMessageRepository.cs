using Consolidado.Domain.Entities;

namespace Consolidado.Application.Interfaces;

public interface IProcessedMessageRepository
{
    Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken);
    Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken);
}
