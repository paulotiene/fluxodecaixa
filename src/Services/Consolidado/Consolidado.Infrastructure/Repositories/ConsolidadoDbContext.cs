using Consolidado.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Consolidado.Infrastructure.Repositories;

public sealed class ConsolidadoDbContext : DbContext
{
    public ConsolidadoDbContext(DbContextOptions<ConsolidadoDbContext> options) : base(options) { }

    public DbSet<ConsolidadoDiario> Consolidados => Set<ConsolidadoDiario>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConsolidadoDiario>(entity =>
        {
            entity.ToTable("ConsolidadosDiarios");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Data).IsUnique();
            entity.Property(x => x.Data).IsRequired();
            entity.Property(x => x.TotalCreditos).HasColumnType("decimal(18,2)");
            entity.Property(x => x.TotalDebitos).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Saldo).HasColumnType("decimal(18,2)");
            entity.Property(x => x.AtualizadoEmUtc).IsRequired();
        });

        modelBuilder.Entity<ProcessedMessage>(entity =>
        {
            entity.ToTable("ProcessedMessages");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.EventId).IsUnique();
            entity.Property(x => x.ProcessedOnUtc).IsRequired();
        });
    }
}
