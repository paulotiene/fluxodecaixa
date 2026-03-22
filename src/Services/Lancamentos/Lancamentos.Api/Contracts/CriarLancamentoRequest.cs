using System.ComponentModel.DataAnnotations;

namespace Lancamentos.Api.Contracts;

public sealed class CriarLancamentoRequest
{
    [Required]
    [Range(1, 2)]
    public int Tipo { get; set; }

    [Required]
    public decimal Valor { get; set; }

    [Required]
    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime DataOcorrencia { get; set; }
}