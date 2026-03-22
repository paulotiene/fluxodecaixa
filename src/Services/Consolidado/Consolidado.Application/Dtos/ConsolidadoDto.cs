namespace Consolidado.Application.Dtos;

public sealed record ConsolidadoDto(
    DateTime Data,
    decimal TotalCreditos,
    decimal TotalDebitos,
    decimal Saldo,
    DateTime AtualizadoEmUtc);
