using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Catalog.API.Contracts.Data;
using Contracts.Pagination;

namespace Catalog.API.Data
{
    public class CatalogRepository<T> : RepositoryBase<T>, ICatalogRepository<T> where T : class, IEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public CatalogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResponse<T>> PaginatedAsync(Specification<T> specification, BaseFilter filter, CancellationToken cancellation)
        {
            var count = await ApplySpecification(specification).CountAsync(cancellation);
            var pagination = new Pagination(count, filter);

            var data = await ApplySpecification(specification)
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToListAsync(cancellation);

            return specification.PostProcessingAction == null
                ? new PagedResponse<T>(data, pagination)
                : new PagedResponse<T>(specification.PostProcessingAction(data).ToList(), pagination);
        }

        public async Task<PagedResponse<TResult>> PaginatedAsync<TResult>(Specification<T, TResult> specification, BaseFilter filter, CancellationToken cancellation)
        {
            var count = await ApplySpecification(specification).CountAsync(cancellation);
            var pagination = new Pagination(count, filter);

            var data = await ApplySpecification(specification)
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToListAsync(cancellation);

            return specification.PostProcessingAction == null
                ? new PagedResponse<TResult>(data, pagination)
                : new PagedResponse<TResult>(specification.PostProcessingAction(data).ToList(), pagination);
        }
    }
}
