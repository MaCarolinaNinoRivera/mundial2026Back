using FantasyWorldCup.Domain.Matches.Entities;
using FantasyWorldCup.Domain.Countries.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class MatchEventConfiguration : IEntityTypeConfiguration<MatchEvent>
{
    public void Configure(EntityTypeBuilder<MatchEvent> builder)
    {
        builder.ToTable("match_events");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(x => x.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(x => x.PlayerId)
            .HasColumnName("player_id")
            .IsRequired();

        builder.Property(x => x.RelatedPlayerId)
            .HasColumnName("related_player_id") // Esto arregla el error de Postgres
            .IsRequired(false);

        builder.Property(x => x.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Minute)
            .HasColumnName("minute")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        // --- RELACIONES ---

        // Un evento pertenece a un partido
        builder.HasOne(x => x.Match)
            .WithMany() // O .WithMany(m => m.Events) si agregas la colección en Match.cs
            .HasForeignKey(x => x.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Un evento pertenece a un jugador
        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para buscar rápido todos los eventos de un partido
        builder.HasIndex(x => x.MatchId)
            .HasDatabaseName("idx_match_events_match_id");
    }
}