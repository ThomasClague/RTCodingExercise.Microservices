using DomainEvents.Messaging;

namespace DomainEvents.Plates;

public abstract record PlateEventBase : AuditableDomainEventBase
{
    public string Registration { get; init; }

    protected PlateEventBase() : base("Plate")
    {
    }
}
