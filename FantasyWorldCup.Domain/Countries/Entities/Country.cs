using FantasyWorldCup.Domain.Common;

namespace FantasyWorldCup.Domain.Countries.Entities;

public class Country : BaseEntity<Guid>
{
    public string Name { get; set; } = null!;
    public Guid? GroupId { get; set; }

    // Propiedad de navegación hacia el Grupo
    public virtual Group? Group { get; set; }

    // Constructor para EF Core
    public Country() { }

    // Constructor para inicializar (opcional)
    public Country(Guid id, string name, Guid? groupId)
    {
        Id = id;
        Name = name;
        GroupId = groupId;
    }
}
