using FantasyWorldCup.Domain.Matches.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class PlayerMatchStatConfiguration : IEntityTypeConfiguration<PlayerMatchStat>
{
    public void Configure(EntityTypeBuilder<PlayerMatchStat> builder)
    {
        // Nombre de la tabla en la base de datos (Postgres)
        builder.ToTable("player_match_stats");

        builder.HasKey(x => x.Id);

        // Mapeo de columnas exactas según tu esquema de Postgres
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.MatchId).HasColumnName("match_id");

        // ¡OJO AQUÍ! En C# es PlayerId, en la DB es football_player_id
        builder.Property(x => x.PlayerId).HasColumnName("football_player_id");

        builder.Property(x => x.MinutesPlayed).HasColumnName("minutes_played");
        builder.Property(x => x.Goals).HasColumnName("goals");
        builder.Property(x => x.Assists).HasColumnName("assists");
        builder.Property(x => x.YellowCards).HasColumnName("yellow_cards");
        builder.Property(x => x.RedCards).HasColumnName("red_cards");
        builder.Property(x => x.PenaltySaved).HasColumnName("penalty_saved");
        builder.Property(x => x.CleanSheet).HasColumnName("clean_sheet");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        // Relaciones (Opcional pero recomendado)
        builder.HasOne<Match>()
            .WithMany()
            .HasForeignKey(x => x.MatchId);
    }
}
