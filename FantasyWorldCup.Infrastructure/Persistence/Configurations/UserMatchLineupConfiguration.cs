using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Domain.Matches.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserMatchLineupConfiguration : IEntityTypeConfiguration<UserMatchLineup>
{
    public void Configure(EntityTypeBuilder<UserMatchLineup> builder)
    {
        // Nombre de la tabla
        builder.ToTable("user_match_lineups");

        // Llave primaria
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        // Columnas básicas
        builder.Property(x => x.UserTeamId)
            .HasColumnName("user_team_id")
            .IsRequired();

        builder.Property(x => x.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(x => x.UserTeamPlayerId)
            .HasColumnName("user_team_player_id")
            .IsRequired();

        builder.Property(x => x.IsStarter)
            .HasColumnName("is_starter")
            .HasDefaultValue(false);

        builder.Property(x => x.IsSubstituted)
            .HasColumnName("is_substituted")
            .HasDefaultValue(false);

        builder.Property(x => x.SubstitutedByLineupId)
            .HasColumnName("substituted_by_lineup_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        // --- RELACIONES CORREGIDAS ---

        // Relación con UserTeam (Usando la propiedad de navegación x.UserTeam)
        builder.HasOne(x => x.UserTeam) // <-- CORREGIDO: Antes era HasOne<UserTeam>()
            .WithMany()
            .HasForeignKey(x => x.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con Match
        builder.HasOne<Match>() // Aquí sí se puede usar genérico si NO tienes "public virtual Match Match" en la entidad
            .WithMany()
            .HasForeignKey(x => x.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con UserTeamPlayer
        builder.HasOne(x => x.UserTeamPlayer)
            .WithMany()
            .HasForeignKey(x => x.UserTeamPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // RELACIÓN RECURSIVA
        builder.HasOne(x => x.SubstitutedBy)
            .WithMany()
            .HasForeignKey(x => x.SubstitutedByLineupId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- ÍNDICES ---
        builder.HasIndex(x => new { x.UserTeamId, x.MatchId, x.UserTeamPlayerId })
            .IsUnique()
            .HasDatabaseName("user_match_lineups_unique_player");

        builder.HasIndex(x => new { x.UserTeamId, x.MatchId })
            .HasDatabaseName("idx_lineups_team_match");
    }
}
