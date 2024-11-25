using DomainEvents.Plates;

namespace Contracts.Plates.Events;

public record PlateSoldEvent : PlateEventBase
{
    public decimal SalePrice { get; init; }
}
 