using System;

namespace FantasyWorldCup.Application.Countries.Queries.GetFilters;

// ESTA CLASE TE FALTABA
public class GroupFilterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

// ESTA CLASE TAMBIÉN TE FALTABA
public class CountryFilterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? GroupId { get; set; }
}

// Esta es la que ya tenías
public class PlayerListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Position { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public string CountryName { get; set; } = null!;
    public Guid? GroupId { get; set; }
}
