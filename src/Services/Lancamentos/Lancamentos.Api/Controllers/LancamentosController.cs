using Lancamentos.Api.Contracts;
using Lancamentos.Application.UseCases.CriarLancamento;
using Lancamentos.Application.UseCases.ListarLancamentos;
using Lancamentos.Application.UseCases.ObterLancamentoPorId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lancamentos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/lancamentos")]
public sealed class LancamentosController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromBody] CriarLancamentoRequest request,
        [FromServices] CriarLancamentoHandler handler,
        CancellationToken cancellationToken)
    {
        var id = await handler.HandleAsync(
            new CriarLancamentoCommand
            {
                Tipo = request.Tipo,
                Valor = request.Valor,
                Descricao = request.Descricao,
                DataOcorrencia = request.DataOcorrencia
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] ObterLancamentoPorIdHandler handler,
        CancellationToken cancellationToken)
    {
        var item = await handler.HandleAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromServices] ListarLancamentosHandler handler,
        CancellationToken cancellationToken)
    {
        var items = await handler.HandleAsync(cancellationToken);
        return Ok(items);
    }
}
