namespace Audit.API.Models.DTOs
{
    public class AuditRecordDto
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
        public string? InitiatedBy { get; set; }
        public string? Description { get; set; }
    }
} 