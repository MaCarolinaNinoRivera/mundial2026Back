using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Teams.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserTeamConfiguration : IEntityTypeConfiguration<UserTeam>
{
    public void Configure(EntityTypeBuilder<UserTeam> builder)
    {
        builder.ToTable("user_teams", "public");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.TotalBudget).HasColumnName("total_budget").IsRequired();
        builder.Property(e => e.AvailableBudget).HasColumnName("available_budget").IsRequired(); // La que agregamos
        builder.Property(e => e.Locked).HasColumnName("locked").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        builder.Property(e => e.ShirtPrimaryColor).HasColumnName("shirt_primary_color");
        builder.Property(e => e.ShirtSecondaryColor).HasColumnName("shirt_secondary_color");
    }
}
