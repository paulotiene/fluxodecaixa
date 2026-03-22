using Lancamentos.Application.Intefaces;

namespace Lancamentos.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly LancamentosDbContext _dbContext;

    public UnitOfWork(LancamentosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
