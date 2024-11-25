using Contracts.Primitives;

namespace WebMVC.Services
{
    public interface IAuditService
    {
        Task<Result<decimal>> GetTotalRevenueAsync(CancellationToken ct = default);
    }
} 