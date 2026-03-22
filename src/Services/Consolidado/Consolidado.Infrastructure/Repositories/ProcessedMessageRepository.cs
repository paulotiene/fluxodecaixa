using Consolidado.Application.Interfaces;
using Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Consolidado.Infrastructure.Repositories;

public sealed class ProcessedMessageRepository : IProcessedMessageRepository
{
    private readonly ConsolidadoDbContext _dbContext;

    public ProcessedMessageRepository(ConsolidadoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid eventId, CancellationToken cancellationToken)
        => _dbContext.ProcessedMessages.AnyAsync(x => x.EventId == eventId, cancellationToken);

    public async Task AddAsync(ProcessedMessage processedMessage, CancellationToken cancellationToken)
        => await _dbContext.ProcessedMessages.AddAsync(processedMessage, cancellationToken);
}
