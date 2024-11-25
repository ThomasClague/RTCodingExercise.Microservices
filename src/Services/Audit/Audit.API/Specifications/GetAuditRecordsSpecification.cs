using Ardalis.Specification;
using Audit.API.Models;
using Audit.API.Models.DTOs;
using Audit.API.Models.Filters;

namespace Audit.API.Specifications
{
    public class GetAuditRecordsSpecification : Specification<AuditRecord, AuditRecordDto>
    {
        public GetAuditRecordsSpecification(AuditRecordFilter filter)
        {
            Query.Select(x => new AuditRecordDto
            {
                Id = x.Id,
                EventType = x.EventType,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Data = x.Data,
                Timestamp = x.Timestamp,
                InitiatedBy = x.InitiatedBy,
                Description = x.Description
            });

            ApplyFilters(filter);
            ApplySorting(filter);
        }

        private void ApplyFilters(AuditRecordFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.EntityType))
                Query.Where(x => x.EntityType == filter.EntityType);

            if (filter.EntityId.HasValue)
                Query.Where(x => x.EntityId == filter.EntityId);

            if (!string.IsNullOrWhiteSpace(filter.EventType))
                Query.Where(x => x.EventType == filter.EventType);

            if (filter.FromDate.HasValue)
                Query.Where(x => x.Timestamp >= filter.FromDate);

            if (filter.ToDate.HasValue)
                Query.Where(x => x.Timestamp <= filter.ToDate);

            if (!string.IsNullOrWhiteSpace(filter.InitiatedBy))
                Query.Where(x => x.InitiatedBy == filter.InitiatedBy);
        }

        private void ApplySorting(AuditRecordFilter filter)
        {
            var isAscending = filter.OrderBy?.ToLower() != "desc";

            if (string.IsNullOrWhiteSpace(filter.SortBy))
            {
                Query.OrderByDescending(x => x.Timestamp);
                return;
            }

            _ = filter.SortBy.ToLower() switch
            {
                "timestamp" => isAscending 
                    ? Query.OrderBy(x => x.Timestamp) 
                    : Query.OrderByDescending(x => x.Timestamp),
                
                "eventtype" => isAscending 
                    ? Query.OrderBy(x => x.EventType) 
                    : Query.OrderByDescending(x => x.EventType),
                
                "entitytype" => isAscending 
                    ? Query.OrderBy(x => x.EntityType) 
                    : Query.OrderByDescending(x => x.EntityType),
                
                "initiatedby" => isAscending 
                    ? Query.OrderBy(x => x.InitiatedBy) 
                    : Query.OrderByDescending(x => x.InitiatedBy),
                
                _ => Query.OrderByDescending(x => x.Timestamp)
            };
        }
    }
} 