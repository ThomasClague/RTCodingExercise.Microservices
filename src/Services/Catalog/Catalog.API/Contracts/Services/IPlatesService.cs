using Contracts.Pagination;
using Contracts.Plates;
using Contracts.Primitives;

namespace Catalog.API.Contracts.Services
{
    public interface IPlatesService
    {
        Task<PagedResponse<Plate>> GetPlatesAsync(PlatesFilter platesFilter, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Result<Plate>> CreatePlateAsync(CreatePlateCommand command, CancellationToken cancellationToken = default);
        Task<Result<Plate>> ReservePlateAsync(Guid plateId, CancellationToken cancellationToken = default);
        Task<Result<Plate>> BuyPlateAsync(Guid plateId, string? discountCode = null, CancellationToken cancellationToken = default);
        Task<decimal> GetAverageProfitMarginAsync(CancellationToken cancellationToken = default);
    }
}
