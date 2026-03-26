using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Teams.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserTeamPlayerConfiguration : IEntityTypeConfiguration<UserTeamPlayer>
{
    public void Configure(EntityTypeBuilder<UserTeamPlayer> builder)
    {
        builder.ToTable("user_team_players", "public");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
        builder.Property(e => e.UserTeamId).HasColumnName("user_team_id").IsRequired();
        builder.Property(e => e.FootballPlayerId).HasColumnName("football_player_id").IsRequired();
        builder.Property(e => e.IsStarter).HasColumnName("is_starter").HasDefaultValue(true);
        builder.Property(e => e.IsCaptain).HasColumnName("is_captain").HasDefaultValue(false);
        builder.Property(e => e.PositionSlot).HasColumnName("position_slot");
        builder.Property(e => e.LockedAt).HasColumnName("locked_at");

        // Relaciones
        builder.HasOne(tp => tp.Player)
            .WithMany()
            .HasForeignKey(tp => tp.FootballPlayerId);
    }
}
