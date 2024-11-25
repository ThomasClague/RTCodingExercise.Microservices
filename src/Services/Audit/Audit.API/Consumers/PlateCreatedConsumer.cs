using Audit.API.Data;
using Audit.API.Models;
using DomainEvents.Plates;
using MassTransit;
using Newtonsoft.Json;

namespace Audit.API.Consumers
{
    public class PlateCreatedConsumer : IConsumer<PlateCreatedEvent> 
    {
        private readonly AuditDbContext _dbContext;
        private readonly ILogger<PlateCreatedConsumer> _logger;

        public PlateCreatedConsumer(
            AuditDbContext dbContext,
            ILogger<PlateCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PlateCreatedEvent> context)
        {
            var @event = context.Message;
            var eventType = nameof(PlateCreatedEvent);

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