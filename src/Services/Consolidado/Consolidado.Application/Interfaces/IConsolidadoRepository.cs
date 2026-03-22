using Consolidado.Application.Dtos;
using Consolidado.Domain.Entities;

namespace Consolidado.Application.Interfaces;

public interface IConsolidadoRepository
{
    Task<ConsolidadoDiario?> GetByDateAsync(DateTime data, CancellationToken cancellationToken);
    Task AddAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken);
    Task<ConsolidadoDto?> GetDtoByDateAsync(DateTime data, CancellationToken cancellationToken);
}
