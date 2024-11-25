namespace DomainEvents.Messaging
{
    public abstract record DomainEventBase : IDomainEvent
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; protected set; } = DateTime.UtcNow;
    }
}
