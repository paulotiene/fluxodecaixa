using Lancamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lancamentos.Infrastructure.Repositories;

public sealed class LancamentosDbContext : DbContext
{
    public LancamentosDbContext(DbContextOptions<LancamentosDbContext> options) : base(options) { }

    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity.ToTable("Lancamentos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descricao).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Tipo).IsRequired();
            entity.Property(x => x.DataOcorrencia).IsRequired();
            entity.Property(x => x.CriadoEmUtc).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Type).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Payload).HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.OccurredOnUtc).IsRequired();
            entity.Property(x => x.Error).HasMaxLength(1000);
        });
    }
}
