using Audit.API.Models.DTOs;
using Audit.API.Models.Filters;
using Contracts.Pagination;

namespace Audit.API.Services
{
    public interface IAuditService
    {
        Task<PagedResponse<AuditRecordDto>> GetAuditRecordsAsync(AuditRecordFilter filter, CancellationToken cancellationToken);
        Task<AuditRecordDto?> GetAuditRecordByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);
    }
} 