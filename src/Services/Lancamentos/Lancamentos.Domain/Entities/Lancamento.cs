using Lancamentos.Domain.Enums;

namespace Lancamentos.Domain.Entities;

public class Lancamento
{
    public Guid Id { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataOcorrencia { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }

    private Lancamento() { }

    public Lancamento(TipoLancamento tipo, decimal valor, string descricao, DateTime dataOcorrencia)
    {
        if (!Enum.IsDefined(tipo))
            throw new ArgumentException("Tipo de lançamento inválido.", nameof(tipo));

        if (tipo != TipoLancamento.Credito && tipo != TipoLancamento.Debito)
            throw new ArgumentException("Tipo de lançamento inválido.", nameof(tipo));

        if (valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(valor));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição é obrigatória.", nameof(descricao));

        if (dataOcorrencia > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException("Data futura inválida.", nameof(dataOcorrencia));

        Id = Guid.NewGuid();
        Tipo = tipo;
        Valor = decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
        Descricao = descricao.Trim();
        DataOcorrencia = dataOcorrencia;
        CriadoEmUtc = DateTime.UtcNow;
    }
}
