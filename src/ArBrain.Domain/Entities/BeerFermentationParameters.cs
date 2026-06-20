namespace ArBrain.Domain.Entities;

/// <summary>
/// Faixas aceitáveis de temperatura, pH e extrato para monitoramento fermentativo.
/// Relacionamento 1:1 com <see cref="Beer"/>.
/// </summary>
public class BeerFermentationParameters
{
    public Guid Id { get; set; }

    public Guid BeerId { get; set; }

    public decimal MinTemperature { get; set; }

    public decimal MaxTemperature { get; set; }

    public decimal MinPh { get; set; }

    public decimal MaxPh { get; set; }

    public decimal MinExtract { get; set; }

    public decimal MaxExtract { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Beer Beer { get; set; } = null!;
}
