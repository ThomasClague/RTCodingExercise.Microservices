using Catalog.API.Contracts.Data;
using Catalog.Domain.Primitives;
using DomainEvents.Messaging;
using MassTransit;

namespace Catalog.API.Services
{
    public class MassTransitDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublishEndpoint _publisher;

        public MassTransitDomainEventDispatcher(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents)
        {
            foreach (EntityBase entitiesWithEvent in entitiesWithEvents)
            {
                foreach (DomainEventBase notification in entitiesWithEvent.DomainEvents)
                {
                    await _publisher.Publish(notification, notification.GetType()).ConfigureAwait(continueOnCapturedContext: false);
                }

                entitiesWithEvent.ClearDomainEvents();
            }
        }
    }
}
