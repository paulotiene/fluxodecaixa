using Lancamentos.Application.Intefaces;
using Lancamentos.Application.Dtos;

namespace Lancamentos.Application.UseCases.ObterLancamentoPorId;

public sealed class ObterLancamentoPorIdHandler
{
    private readonly ILancamentoRepository _lancamentoRepository;

    public ObterLancamentoPorIdHandler(ILancamentoRepository lancamentoRepository)
    {
        _lancamentoRepository = lancamentoRepository;
    }

    public async Task<LancamentoListItemDto?> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var lancamento = await _lancamentoRepository.GetByIdAsync(id, cancellationToken);
        if (lancamento is null) return null;

        return new LancamentoListItemDto(
            lancamento.Id,
            (int)lancamento.Tipo,
            lancamento.Valor,
            lancamento.Descricao,
            lancamento.DataOcorrencia,
            lancamento.CriadoEmUtc);
    }
}
