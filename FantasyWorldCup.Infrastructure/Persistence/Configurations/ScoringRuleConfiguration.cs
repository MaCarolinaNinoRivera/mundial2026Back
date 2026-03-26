using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Scoring.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class ScoringRuleConfiguration : IEntityTypeConfiguration<ScoringRule>
{
    public void Configure(EntityTypeBuilder<ScoringRule> builder)
    {
        // Forzamos el nombre de la tabla en minúsculas
        builder.ToTable("scoring_rules", "public");

        builder.HasKey(e => e.Id);

        // MAPEO EXPLÍCITO DE COLUMNAS A MINÚSCULAS (SNAKE_CASE)
        builder.Property(e => e.Id)
            .HasColumnName("id"); // <--- ESTO SOLUCIONA EL ERROR "column s.Id does not exist"

        builder.Property(e => e.RuleKey)
            .HasColumnName("rule_key")
            .IsRequired();

        builder.Property(e => e.PositionType)
            .HasColumnName("position_type");

        builder.Property(e => e.PointsValue)
            .HasColumnName("points_value")
            .IsRequired();

        // Agregamos description que está en tu DB
        builder.Property(e => e.Description)
            .HasColumnName("description");
    }
}
