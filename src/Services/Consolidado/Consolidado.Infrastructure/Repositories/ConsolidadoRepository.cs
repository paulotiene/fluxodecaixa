using Consolidado.Application.Interfaces;
using Consolidado.Application.Dtos;
using Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Consolidado.Infrastructure.Repositories;

public sealed class ConsolidadoRepository : IConsolidadoRepository
{
    private readonly ConsolidadoDbContext _dbContext;

    public ConsolidadoRepository(ConsolidadoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ConsolidadoDiario?> GetByDateAsync(DateTime data, CancellationToken cancellationToken)
        => _dbContext.Consolidados.FirstOrDefaultAsync(x => x.Data == data.Date, cancellationToken);

    public async Task AddAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken)
        => await _dbContext.Consolidados.AddAsync(consolidado, cancellationToken);

    public async Task<ConsolidadoDto?> GetDtoByDateAsync(DateTime data, CancellationToken cancellationToken)
        => await _dbContext.Consolidados
            .AsNoTracking()
            .Where(x => x.Data == data.Date)
            .Select(x => new ConsolidadoDto(
                x.Data,
                x.TotalCreditos,
                x.TotalDebitos,
                x.Saldo,
                x.AtualizadoEmUtc))
            .FirstOrDefaultAsync(cancellationToken);
}
