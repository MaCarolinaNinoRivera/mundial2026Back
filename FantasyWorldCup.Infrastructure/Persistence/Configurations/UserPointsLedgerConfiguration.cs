using FantasyWorldCup.Domain.Scoring.Entities; // Verifica si tu entidad está en Scoring o Teams
using FantasyWorldCup.Domain.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserPointsLedgerConfiguration : IEntityTypeConfiguration<UserPointsLedger>
{
    public void Configure(EntityTypeBuilder<UserPointsLedger> builder)
    {
        // 1. Nombre de la tabla
        builder.ToTable("user_points_ledger", "public");

        // 2. Llave primaria
        builder.HasKey(x => x.Id);

        // 3. Mapeo de columnas (Snake Case)
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.SourceType).HasColumnName("source_type");
        builder.Property(x => x.SourceId).HasColumnName("source_id");
        builder.Property(x => x.Points).HasColumnName("points");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        // 4. Relación con el usuario (Opcional pero recomendado)
        builder.HasOne<User>()
           .WithMany()
           .HasForeignKey(x => x.UserId);
    }
}
