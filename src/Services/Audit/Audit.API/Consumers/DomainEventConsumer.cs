using Audit.API.Data;
using Audit.API.Models;
using DomainEvents.Messaging;
using MassTransit;
using Newtonsoft.Json;

namespace Audit.API.Consumers
{
    public class DomainEventConsumer<TEvent> : IConsumer<TEvent> 
        where TEvent : AuditableDomainEventBase
    {
        private readonly AuditDbContext _dbContext;
        private readonly ILogger<DomainEventConsumer<TEvent>> _logger;

        public DomainEventConsumer(
            AuditDbContext dbContext,
            ILogger<DomainEventConsumer<TEvent>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TEvent> context)
        {
            var @event = context.Message;
            var eventType = typeof(TEvent).Name;
            
            _logger.LogInformation("Received auditable event {EventType}", eventType);

            var auditRecord = new AuditRecord
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                EntityType = @event.EntityType,
                EntityId = @event.Id,
                Data = JsonConvert.SerializeObject(@event),
                Timestamp = @event.DateCreated,
                InitiatedBy = @event.InitiatedBy,
                Description = @event.Description
            };

            await _dbContext.AuditRecords.AddAsync(auditRecord);
            await _dbContext.SaveChangesAsync();
        }
    }
} 