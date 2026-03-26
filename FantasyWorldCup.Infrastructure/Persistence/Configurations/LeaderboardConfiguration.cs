using FantasyWorldCup.Application.Scoring.Queries.GetLeaderboard; // <-- IMPORTANTE: Referencia a la capa Application
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class LeaderboardConfiguration : IEntityTypeConfiguration<LeaderboardDto>
{
    public void Configure(EntityTypeBuilder<LeaderboardDto> builder)
    {
        builder.HasNoKey(); // Las vistas no suelen tener PK
        builder.ToTable("leaderboard"); // Nombre de la tabla/vista en Postgres

        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.Username).HasColumnName("username");
        builder.Property(e => e.TotalPoints).HasColumnName("total_points");
    }
}
