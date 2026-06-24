namespace ArBrain.Domain.Entities;

/// <summary>
/// Tanque de fermentação utilizado nos apontamentos.
/// </summary>
public class Tank
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Capacidade total do tanque em litros.</summary>
    public decimal CapacityLiters { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<FermentationRecord> FermentationRecords { get; set; } = [];
}
