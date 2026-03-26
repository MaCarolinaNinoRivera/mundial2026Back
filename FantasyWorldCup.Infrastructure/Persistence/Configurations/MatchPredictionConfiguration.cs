using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Predictions.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class MatchPredictionConfiguration : IEntityTypeConfiguration<MatchPrediction>
{
    public void Configure(EntityTypeBuilder<MatchPrediction> builder)
    {
        builder.ToTable("match_predictions", "public");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(e => e.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(e => e.PredictedHomeGoals)
            .HasColumnName("predicted_home_goals")
            .IsRequired();

        builder.Property(e => e.PredictedAwayGoals)
            .HasColumnName("predicted_away_goals")
            .IsRequired();

        builder.Property(e => e.LockedAt)
            .HasColumnName("locked_at");

        builder.Property(e => e.PointsEarned)
            .HasColumnName("points_earned")
            .HasDefaultValue(0);
    }
}
