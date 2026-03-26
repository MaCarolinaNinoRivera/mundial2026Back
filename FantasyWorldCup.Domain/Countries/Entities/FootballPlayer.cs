using FantasyWorldCup.Domain.Countries.Entities;

namespace FantasyWorldCup.Domain.Countries.Entities;

public class FootballPlayer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty; // Forward, Midfielder, etc.
    public Guid CountryId { get; set; }
    public decimal BasePrice { get; set; } // Mapea a base_price
    public bool IsActive { get; set; }     // Mapea a is_active
    public int ShirtNumber { get; set; }

    // Relación con el país
    public Country Country { get; set; } = default!;
}
