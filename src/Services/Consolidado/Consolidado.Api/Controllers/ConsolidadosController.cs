using Consolidado.Application.UseCases.ObterConsolidadoPorData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consolidado.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/consolidados")]
public sealed class ConsolidadosController : ControllerBase
{
    [HttpGet("{data}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByDate(
        string data,
        [FromServices] ObterConsolidadoPorDataHandler handler,
        CancellationToken cancellationToken)
    {
        if (!DateTime.TryParse(data, out var parsedDate))
        {
            return BadRequest("Data inválida. Use yyyy-MM-dd.");
        }

        var result = await handler.HandleAsync(parsedDate.Date, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
