using Contracts.Pagination;

namespace Audit.API.Models.Filters
{
    public class AuditRecordFilter : BaseFilter
    {
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? EventType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? InitiatedBy { get; set; }
    }
} 