using FantasyWorldCup.Domain.Teams.Entities;
using FantasyWorldCup.Domain.Matches.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserMatchSubstitutionConfiguration : IEntityTypeConfiguration<UserMatchSubstitution>
{
    public void Configure(EntityTypeBuilder<UserMatchSubstitution> builder)
    {
        // Nombre de la tabla en Postgres
        builder.ToTable("user_match_substitutions");

        // Llave primaria
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        // Columnas básicas y de tiempo
        builder.Property(x => x.UserTeamId)
            .HasColumnName("user_team_id")
            .IsRequired();

        builder.Property(x => x.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(x => x.LineupPlayerOutId)
            .HasColumnName("lineup_player_out_id")
            .IsRequired();

        builder.Property(x => x.LineupPlayerInId)
            .HasColumnName("lineup_player_in_id")
            .IsRequired();

        builder.Property(x => x.SubstitutionTime)
            .HasColumnName("substitution_time")
            .IsRequired();

        builder.Property(x => x.SubstitutionMinute)
            .HasColumnName("substitution_minute")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        // --- RELACIONES CORREGIDAS PARA EVITAR "Id1" ---

        // Relación con el jugador de la alineación que SALE
        builder.HasOne(x => x.LineupPlayerOut) // <--- Cambiado de genérico a propiedad de navegación
            .WithMany()
            .HasForeignKey(x => x.LineupPlayerOutId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_sub_player_out");

        // Relación con el jugador de la alineación que ENTRA
        builder.HasOne(x => x.LineupPlayerIn) // <--- Vinculación explícita
            .WithMany()
            .HasForeignKey(x => x.LineupPlayerInId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_sub_player_in");

        // Relación con el equipo del usuario
        builder.HasOne<UserTeam>()
            .WithMany()
            .HasForeignKey(x => x.UserTeamId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con el partido
        builder.HasOne<Match>()
            .WithMany()
            .HasForeignKey(x => x.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- ÍNDICES ---
        builder.HasIndex(x => new { x.UserTeamId, x.MatchId })
            .HasDatabaseName("idx_substitutions_team_match");
    }
}
