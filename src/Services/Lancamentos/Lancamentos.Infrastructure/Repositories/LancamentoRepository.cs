using Lancamentos.Application.Intefaces;
using Lancamentos.Application.Dtos;
using Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lancamentos.Infrastructure.Repositories;

public sealed class LancamentoRepository : ILancamentoRepository
{
    private readonly LancamentosDbContext _dbContext;

    public LancamentoRepository(LancamentosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken)
        => await _dbContext.Lancamentos.AddAsync(lancamento, cancellationToken);

    public Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Lancamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LancamentoListItemDto>> GetAllAsync(CancellationToken cancellationToken)
        => await _dbContext.Lancamentos
            .AsNoTracking()
            .OrderByDescending(x => x.DataOcorrencia)
            .Select(x => new LancamentoListItemDto(
                x.Id,
                (int)x.Tipo,
                x.Valor,
                x.Descricao,
                x.DataOcorrencia,
                x.CriadoEmUtc))
            .ToListAsync(cancellationToken);
}
