using WebMVC.Models;
using Contracts.Primitives;
using Contracts.Plates;

namespace WebMVC.Services
{
    public interface ICatalogService
    {
        Task<Result<CreatePlateViewModel>> CreatePlateAsync(CreatePlateViewModel model);
        Task<Result> ReservePlateAsync(Guid plateId);
        Task<Result> BuyPlateAsync(Guid plateId, string? discountCode);
        Task<Result<PlatesViewModel>> GetPlatesAsync(PlatesFilter platesFilter, bool isAdmin, CancellationToken ct = default);
        Task<Result<decimal>> GetAverageProfitMarginAsync(CancellationToken ct = default);
    }
} 