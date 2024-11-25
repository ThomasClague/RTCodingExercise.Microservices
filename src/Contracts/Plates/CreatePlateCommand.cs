namespace Contracts.Plates;

public record CreatePlateCommand
{
    public string Registration { get; init; }
    public decimal PurchasePrice { get; init; }
    public decimal SalePrice { get; init; }
} 