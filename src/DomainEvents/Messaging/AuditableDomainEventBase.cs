
namespace DomainEvents.Messaging
{
    public abstract record AuditableDomainEventBase : DomainEventBase
    {
        public string EntityType { get; private init; }
        public string? InitiatedBy { get; init; }
        public string? Description { get; init; }

        protected AuditableDomainEventBase(string entityType)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("Entity type cannot be null or empty", nameof(entityType));
                
            EntityType = entityType;
        }
    }
} 