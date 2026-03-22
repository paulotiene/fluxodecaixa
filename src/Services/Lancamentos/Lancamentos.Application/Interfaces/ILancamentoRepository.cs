using Lancamentos.Application.Dtos;
using Lancamentos.Domain.Entities;

namespace Lancamentos.Application.Intefaces;

public interface ILancamentoRepository
{
    Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken);
    Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<LancamentoListItemDto>> GetAllAsync(CancellationToken cancellationToken);
}
