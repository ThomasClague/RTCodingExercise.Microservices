using Contracts.Plates;
using Contracts.Primitives;
using MassTransit;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestClient<CreatePlateRequest> _createPlateClient;
        private readonly IRequestClient<ReservePlateRequest> _reservePlateClient;
        private readonly IRequestClient<BuyPlateRequest> _buyPlateClient;
        private readonly IRequestClient<GetPlatesRequest> _getPlatesClient;
        private readonly IRequestClient<GetAverageProfitMarginRequest> _getAverageProfitMarginClient;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(
            IRequestClient<CreatePlateRequest> createPlateClient,
            IRequestClient<ReservePlateRequest> reservePlateClient,
            IRequestClient<BuyPlateRequest> buyPlateClient,
            IRequestClient<GetPlatesRequest> getPlatesClient,
            IRequestClient<GetAverageProfitMarginRequest> getAverageProfitMarginClient,
            ILogger<CatalogService> logger)
        {
            _createPlateClient = createPlateClient;
            _reservePlateClient = reservePlateClient;
            _buyPlateClient = buyPlateClient;
            _getPlatesClient = getPlatesClient;
            _getAverageProfitMarginClient = getAverageProfitMarginClient;
            _logger = logger;
        }

        public async Task<Result<PlatesViewModel>> GetPlatesAsync(
            PlatesFilter platesFilter,
            bool isAdmin,
            CancellationToken ct = default)
        {
            try
            {
                var response = await _getPlatesClient.GetResponse<GetPlatesResponse>(
                    new GetPlatesRequest(platesFilter, isAdmin),
                    ct);

                var viewModel = new PlatesViewModel
                {
                    PagedResponse = response.Message.PagedResponse,
                    PlatesFilter = platesFilter,
                };

                return Result<PlatesViewModel>.Success(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting plates");
                return Result<PlatesViewModel>.Failure("An error occurred while getting the plates");
            }
        }

        public async Task<Result<CreatePlateViewModel>> CreatePlateAsync(CreatePlateViewModel model)
        {
            try
            {
                var command = new CreatePlateCommand
                {
                    Registration = model.Registration,
                    PurchasePrice = model.PurchasePrice,
                    SalePrice = model.SalePrice
                };

                var response = await _createPlateClient.GetResponse<CreatePlateResponse, CreatePlateFailureResponse>(
                    new CreatePlateRequest(command));

                if (response.Is(out Response<CreatePlateResponse> successResponse))
                {
                    var createdPlate = new CreatePlateViewModel
                    {
                        Registration = successResponse.Message.Plate.Registration,
                        PurchasePrice = successResponse.Message.Plate.PurchasePrice,
                        SalePrice = successResponse.Message.Plate.DisplayPrice
                    };

                    return Result<CreatePlateViewModel>.Success(createdPlate);
                }
                else if (response.Is(out Response<CreatePlateFailureResponse> failureResponse))
                {
                    return Result<CreatePlateViewModel>.Failure(failureResponse.Message.Errors);
                }

                return Result<CreatePlateViewModel>.Failure("Unexpected response type received");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plate");
                return Result<CreatePlateViewModel>.Failure("An error occurred while creating the plate");
            }
        }

        public async Task<Result> ReservePlateAsync(Guid plateId)
        {
            try
            {
                var response = await _reservePlateClient.GetResponse<ReservePlateResponse, ReservePlateFailureResponse>(
                    new ReservePlateRequest { PlateId = plateId });

                if (response.Is(out Response<ReservePlateResponse> successResponse))
                {
                    return Result.Success();
                }
                else if (response.Is(out Response<ReservePlateFailureResponse> failureResponse))
                {
                    return Result.Failure(failureResponse.Message.Errors);
                }

                return Result.Failure("Unexpected response type received");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving plate");
                return Result.Failure("An error occurred while reserving the plate");
            }
        }

        public async Task<Result> BuyPlateAsync(Guid plateId, string? discountCode)
        {
            try
            {
                var response = await _buyPlateClient.GetResponse<BuyPlateResponse, BuyPlateFailureResponse>(
                    new BuyPlateRequest { PlateId = plateId, DiscountCode = discountCode });

                if (response.Is(out Response<BuyPlateResponse> successResponse))
                {
                    return Result.Success();
                }
                else if (response.Is(out Response<BuyPlateFailureResponse> failureResponse))
                {
                    return Result.Failure(failureResponse.Message.Errors);
                }

                return Result.Failure("Unexpected response type received");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buying plate");
                return Result.Failure("An error occurred while buying the plate");
            }
        }

        public async Task<Result<decimal>> GetAverageProfitMarginAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _getAverageProfitMarginClient.GetResponse<GetAverageProfitMarginResponse>(
                    new GetAverageProfitMarginRequest(),
                    ct);

                return Result<decimal>.Success(response.Message.AverageProfitMargin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average profit margin");
                return Result<decimal>.Failure("An error occurred while getting the average profit margin");
            }
        }
    }
} 