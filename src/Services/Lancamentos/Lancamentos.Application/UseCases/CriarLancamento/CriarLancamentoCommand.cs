namespace Lancamentos.Application.UseCases.CriarLancamento;

public sealed class CriarLancamentoCommand
{
    public int Tipo { get; init; }
    public decimal Valor { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public DateTime DataOcorrencia { get; init; }
}
