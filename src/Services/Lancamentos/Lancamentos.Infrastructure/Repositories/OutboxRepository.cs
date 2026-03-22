using Lancamentos.Application.Intefaces;
using Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lancamentos.Infrastructure.Repositories;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly LancamentosDbContext _dbContext;

    public OutboxRepository(LancamentosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken)
        => await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);

    public async Task<IReadOnlyList<OutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken)
        => await _dbContext.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
}
