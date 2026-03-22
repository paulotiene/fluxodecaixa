using Lancamentos.Application.Intefaces;
using Lancamentos.Application.Dtos;

namespace Lancamentos.Application.UseCases.ListarLancamentos;

public sealed class ListarLancamentosHandler
{
    private readonly ILancamentoRepository _lancamentoRepository;

    public ListarLancamentosHandler(ILancamentoRepository lancamentoRepository)
    {
        _lancamentoRepository = lancamentoRepository;
    }

    public Task<IReadOnlyList<LancamentoListItemDto>> HandleAsync(CancellationToken cancellationToken)
        => _lancamentoRepository.GetAllAsync(cancellationToken);
}
