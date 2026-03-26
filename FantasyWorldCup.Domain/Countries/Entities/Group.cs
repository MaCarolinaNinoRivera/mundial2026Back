using FantasyWorldCup.Domain.Common;

namespace FantasyWorldCup.Domain.Countries.Entities;

// Añadimos <Guid> a la herencia
public class Group : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;

    // Relación inversa: Un grupo tiene muchos países
    public ICollection<Country> Countries { get; set; } = new List<Country>();
}
