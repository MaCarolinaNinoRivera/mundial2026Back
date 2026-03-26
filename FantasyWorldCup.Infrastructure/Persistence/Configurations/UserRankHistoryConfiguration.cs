using FantasyWorldCup.Domain.Scoring.Entities;
using FantasyWorldCup.Domain.Auth.Entities; // <-- NUEVO: Para que reconozca a 'User'
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class UserRankHistoryConfiguration : IEntityTypeConfiguration<UserRankHistory>
{
    public void Configure(EntityTypeBuilder<UserRankHistory> builder)
    {
        builder.ToTable("user_rank_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.RankPosition)
            .HasColumnName("rank_position")
            .IsRequired();

        builder.Property(x => x.TotalPoints)
            .HasColumnName("total_points")
            .IsRequired();

        builder.Property(x => x.CalculatedAt)
            .HasColumnName("calculated_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // Ahora esto ya no dará error porque añadimos el using arriba
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}
