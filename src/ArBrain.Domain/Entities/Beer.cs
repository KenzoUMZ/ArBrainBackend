using ArBrain.Domain.Enums;

namespace ArBrain.Domain.Entities;

/// <summary>
/// Representa uma cerveja cadastrada no sistema (nome e estilo).
/// </summary>
public class Beer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public BeerStyle Style { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    /// <summary>Parâmetros fermentativos aceitáveis vinculados à cerveja.</summary>
    public BeerFermentationParameters? FermentationParameters { get; set; }

    public ICollection<FermentationRecord> FermentationRecords { get; set; } = [];
}
