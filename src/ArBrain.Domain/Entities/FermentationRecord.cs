using ArBrain.Domain.Enums;

namespace ArBrain.Domain.Entities;

/// <summary>
/// Apontamento fermentativo registrado durante o processo de produção.
/// </summary>
public class FermentationRecord
{
    public Guid Id { get; set; }

    /// <summary>Data e hora em que a medição foi realizada.</summary>
    public DateTime RegisteredAt { get; set; }

    public Guid BeerId { get; set; }

    public Guid TankId { get; set; }

    /// <summary>Identificador do lote (ex.: IPA001).</summary>
    public string BatchNumber { get; set; } = string.Empty;

    public decimal Temperature { get; set; }

    public decimal Ph { get; set; }

    public decimal Extract { get; set; }

    public string? Observations { get; set; }

    /// <summary>Status calculado com base nos parâmetros aceitáveis da cerveja.</summary>
    public FermentationComplianceStatus ComplianceStatus { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Beer Beer { get; set; } = null!;

    public Tank Tank { get; set; } = null!;
}
