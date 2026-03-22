using Consolidado.Application.Interfaces;
using Consolidado.Application.Dtos;

namespace Consolidado.Application.UseCases.ObterConsolidadoPorData;

public sealed class ObterConsolidadoPorDataHandler
{
    private readonly IConsolidadoRepository _consolidadoRepository;

    public ObterConsolidadoPorDataHandler(IConsolidadoRepository consolidadoRepository)
    {
        _consolidadoRepository = consolidadoRepository;
    }

    public Task<ConsolidadoDto?> HandleAsync(DateTime data, CancellationToken cancellationToken)
        => _consolidadoRepository.GetDtoByDateAsync(data.Date, cancellationToken);
}
