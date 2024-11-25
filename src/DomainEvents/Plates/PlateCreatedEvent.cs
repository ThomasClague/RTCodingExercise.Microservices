namespace DomainEvents.Plates;

public record PlateCreatedEvent : PlateEventBase
{
    public decimal PurchasePrice { get; init; }
    public decimal SalePrice { get; init; }
}