namespace Lancamentos.Application.Dtos;

public sealed record LancamentoListItemDto(
    Guid Id,
    int Tipo,
    decimal Valor,
    string Descricao,
    DateTime DataOcorrencia,
    DateTime CriadoEmUtc);
