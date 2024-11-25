using Ardalis.Specification;
using Contracts.Pagination;

namespace Audit.API.Contracts.Data
{
    public interface IAuditRepository<T> : IRepositoryBase<T> where T : class
    {
        Task<PagedResponse<T>> PaginatedAsync(Specification<T> specification, BaseFilter filter, CancellationToken cancellation);
        Task<PagedResponse<TResult>> PaginatedAsync<TResult>(Specification<T, TResult> specification, BaseFilter filter, CancellationToken cancellation);
    }
} 