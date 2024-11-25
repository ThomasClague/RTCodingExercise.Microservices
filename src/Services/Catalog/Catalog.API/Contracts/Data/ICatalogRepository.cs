using Ardalis.Specification;
using Contracts.Pagination;

namespace Catalog.API.Contracts.Data
{
    public interface ICatalogRepository<T> : IRepositoryBase<T> where T : class, IEntity
    {
        Task<PagedResponse<T>> PaginatedAsync(Specification<T> specification, BaseFilter filter, CancellationToken cancellation);
        Task<PagedResponse<TResult>> PaginatedAsync<TResult>(Specification<T, TResult> specification, BaseFilter filter, CancellationToken cancellation);
    }
}
