namespace Consolidado.Domain.Entities;

public class ConsolidadoDiario
{
    public Guid Id { get; private set; }
    public DateTime Data { get; private set; }
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public decimal Saldo { get; private set; }
    public DateTime AtualizadoEmUtc { get; private set; }

    private ConsolidadoDiario() { }

    public ConsolidadoDiario(DateTime data)
    {
        Id = Guid.NewGuid();
        Data = data.Date;
        AtualizadoEmUtc = DateTime.UtcNow;
    }

    public void AplicarLancamento(int tipo, decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(valor));

        if (tipo == 1)
            TotalCreditos += decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
        else if (tipo == 2)
            TotalDebitos += decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
        else
            throw new ArgumentException("Tipo de lançamento inválido.", nameof(tipo));

        Saldo = TotalCreditos - TotalDebitos;
        AtualizadoEmUtc = DateTime.UtcNow;
    }
}
