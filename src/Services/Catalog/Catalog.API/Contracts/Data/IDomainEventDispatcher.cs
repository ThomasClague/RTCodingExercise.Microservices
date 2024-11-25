using Catalog.Domain.Primitives;

namespace Catalog.API.Contracts.Data
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
    }
}
