using Consolidado.Application.Interfaces;

namespace Consolidado.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ConsolidadoDbContext _dbContext;

    public UnitOfWork(ConsolidadoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
