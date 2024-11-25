using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Audit.API.Contracts.Data;
using Contracts.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Audit.API.Data
{
    public class AuditRepository<T> : RepositoryBase<T>, IAuditRepository<T> where T : class
    {
        private readonly AuditDbContext _dbContext;

        public AuditRepository(AuditDbContext dbContext) : base(dbContext)
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

            return new PagedResponse<T>(data, pagination);
        }

        public async Task<PagedResponse<TResult>> PaginatedAsync<TResult>(Specification<T, TResult> specification, BaseFilter filter, CancellationToken cancellation)
        {
            var count = await ApplySpecification(specification).CountAsync(cancellation);
            var pagination = new Pagination(count, filter);

            var data = await ApplySpecification(specification)
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToListAsync(cancellation);

            return new PagedResponse<TResult>(data, pagination);
        }
    }
} 