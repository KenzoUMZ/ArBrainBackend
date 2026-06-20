using ArBrain.Domain.Enums;

namespace ArBrain.Domain.Entities;

public class Beer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public BeerStyle Style { get; set; }

    public decimal Abv { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int MinimumStock { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsLowStock => StockQuantity <= MinimumStock;
}
