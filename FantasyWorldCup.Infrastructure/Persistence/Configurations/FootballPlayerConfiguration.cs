using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class FootballPlayerConfiguration : IEntityTypeConfiguration<FootballPlayer>
{
    public void Configure(EntityTypeBuilder<FootballPlayer> builder)
    {
        builder.ToTable("football_players", "public");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CountryId).HasColumnName("country_id");
        builder.Property(e => e.BasePrice).HasColumnName("base_price");
        builder.Property(e => e.IsActive).HasColumnName("is_active");
        builder.Property(e => e.Name).HasColumnName("name");
        builder.Property(e => e.Position).HasColumnName("position");
        builder.Property(e => e.ShirtNumber).HasColumnName("shirt_number");
        builder.HasOne(d => d.Country)
            .WithMany()
            .HasForeignKey(d => d.CountryId);
    }
}
