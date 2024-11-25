using Audit.API.Contracts.Data;
using Audit.API.Models;
using Audit.API.Models.DTOs;
using Audit.API.Models.Filters;
using Audit.API.Specifications;
using Contracts.Pagination;
using System.Text.Json;

namespace Audit.API.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository<AuditRecord> _repository;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IAuditRepository<AuditRecord> repository, ILogger<AuditService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResponse<AuditRecordDto>> GetAuditRecordsAsync(AuditRecordFilter filter, CancellationToken cancellationToken)
        {
            var spec = new GetAuditRecordsSpecification(filter);
            return await _repository.PaginatedAsync(spec, filter, cancellationToken);
        }

        public async Task<AuditRecordDto?> GetAuditRecordByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var record = await _repository.GetByIdAsync(id, cancellationToken);
            if (record == null) return null;

            return new AuditRecordDto
            {
                Id = record.Id,
                EventType = record.EventType,
                EntityType = record.EntityType,
                EntityId = record.EntityId,
                Data = record.Data,
                Timestamp = record.Timestamp,
                InitiatedBy = record.InitiatedBy,
                Description = record.Description
            };
        }

        public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
        {
            var spec = new GetPlatePurchaseRecordsSpecification();
            var results = await _repository.ListAsync(spec, cancellationToken);
            
            decimal totalRevenue = 0;
            foreach (var jsonData in results)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                    if (data.TryGetProperty("SalePrice", out JsonElement salePriceElement))
                    {
                        if (salePriceElement.TryGetDecimal(out decimal salePrice))
                        {
                            totalRevenue += salePrice;
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing audit record data");
                }
            }
            
            return totalRevenue;
        }
    }
} 