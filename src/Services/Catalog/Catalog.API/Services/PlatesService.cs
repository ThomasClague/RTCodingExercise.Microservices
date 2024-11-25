using Catalog.Domain.Exceptions;
using Catalog.API.Contracts.Services;
using Catalog.API.Specifications.Plates;
using Contracts.Pagination;
using Contracts.Plates;
using MassTransit;
using Contracts.Primitives;
using Catalog.API.Contracts.Data;

namespace Catalog.API.Services
{
    public class PlatesService : IPlatesService
    {
        private readonly ICatalogRepository<Plate> _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public PlatesService(
            ICatalogRepository<Plate> repository,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<PagedResponse<Plate>> GetPlatesAsync(
            PlatesFilter platesFilter,
            bool isAdmin,
            CancellationToken cancellationToken = default)
        {
            var spec = new GetPlatesSpecification(platesFilter, isAdmin);
            return await _repository.PaginatedAsync(spec, platesFilter, cancellationToken);
        }

        public async Task<Result<Plate>> CreatePlateAsync(
            CreatePlateCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var plate = Plate.Create(command.Registration, command.PurchasePrice, command.SalePrice);
                var createdPlate = await _repository.AddAsync(plate, cancellationToken);
                return Result<Plate>.Success(createdPlate);
            }
            catch (InvalidPlateException ex)
            {
                return Result<Plate>.Failure(ex.Errors);
            }
            catch (Exception ex)
            {
                return Result<Plate>.Failure($"Failed to create plate: {ex.Message}");
            }
        }

        public async Task<Result<Plate>> ReservePlateAsync(
            Guid plateId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var plate = await _repository.GetByIdAsync(plateId, cancellationToken);
                
                if (plate == null)
                {
                    return Result<Plate>.Failure($"Plate with ID {plateId} not found");
                }

                plate.Reserve();
                await _repository.UpdateAsync(plate, cancellationToken);
                
                return Result<Plate>.Success(plate);
            }
            catch (PlateAlreadyReservedException ex)
            {
                return Result<Plate>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Plate>.Failure($"Failed to reserve plate: {ex.Message}");
            }
        }

        public async Task<Result<Plate>> BuyPlateAsync(
            Guid plateId,
            string? discountCode,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var plate = await _repository.GetByIdAsync(plateId, cancellationToken);
                
                if (plate == null)
                {
                    return Result<Plate>.Failure($"Plate with ID {plateId} not found");
                }

                if (plate.StatusId == (int)PlateStatusEnum.Sold)
                {
                    return Result<Plate>.Failure($"Plate {plate.Registration} has already been sold");
                }

                plate.MarkAsSold(discountCode);
                await _repository.UpdateAsync(plate, cancellationToken);
                
                return Result<Plate>.Success(plate);
            }
            catch (Exception ex) when (ex is PlateAlreadySoldException ||
                                      ex is InvalidDiscountCodeException ||
                                      ex is DiscountExceedsMinimumPriceException)
            {
                return Result<Plate>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Plate>.Failure($"Failed to buy plate: {ex.Message}");
            }
        }

        public async Task<decimal> GetAverageProfitMarginAsync(CancellationToken cancellationToken = default)
        {
            var spec = new GetAllPlatesSpecification();
            var plates = await _repository.ListAsync(spec, cancellationToken);
            
            if (!plates.Any())
                return 0;

            return CalculateAverageProfitMargin(plates);
        }

        private static decimal CalculateAverageProfitMargin(IEnumerable<Plate> plates)
        {
            var totalProfitMargin = plates.Sum(plate => 
                ((plate.SalePrice - plate.PurchasePrice) / plate.PurchasePrice) * 100);
            
            return Math.Round(totalProfitMargin / plates.Count(), 2);
        }
    }
}
