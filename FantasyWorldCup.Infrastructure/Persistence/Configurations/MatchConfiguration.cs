using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Matches.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("matches", "public");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id");

        builder.Property(m => m.HomeCountryId)
            .HasColumnName("home_country_id")
            .IsRequired();

        builder.Property(m => m.AwayCountryId)
            .HasColumnName("away_country_id")
            .IsRequired();

        builder.Property(m => m.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(m => m.HomeGoals)
            .HasColumnName("home_goals");

        builder.Property(m => m.AwayGoals)
            .HasColumnName("away_goals");

        builder.Property(m => m.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();
    }
}
    