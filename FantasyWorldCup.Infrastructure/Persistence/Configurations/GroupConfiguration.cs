using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        // 1. Mapear al nombre exacto de la tabla en la base de datos (minúsculas)
        builder.ToTable("groups", "public");

        // 2. Definir la Llave Primaria
        builder.HasKey(g => g.Id);

        // 3. Mapear columna 'id' (UUID con valor por defecto)
        builder.Property(g => g.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuid_generate_v4()");

        // 4. Mapear columna 'name'
        builder.Property(g => g.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        // 5. Configurar la relación con Country (Un grupo tiene muchos países)
        builder.HasMany(g => g.Countries)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
