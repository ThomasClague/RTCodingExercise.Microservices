using DomainEvents.Messaging;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Domain.Primitives
{
    public abstract class EntityBase : IEquatable<EntityBase>, IEntity
    {
        public int Id { get; init; }

        private List<DomainEventBase> _domainEvents = new List<DomainEventBase>();

        [NotMapped]
        public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

        public void RegisterDomainEvent(DomainEventBase domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public bool Equals(EntityBase? other)
        {
            return other != null && other.Id == this.Id;
        }
    }

}
