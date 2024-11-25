using Contracts.Audit;
using Contracts.Primitives;
using MassTransit;

namespace WebMVC.Services
{
    public class AuditService : IAuditService
    {
        private readonly IRequestClient<GetTotalRevenueRequest> _getTotalRevenueClient;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IRequestClient<GetTotalRevenueRequest> getTotalRevenueClient,
            ILogger<AuditService> logger)
        {
            _getTotalRevenueClient = getTotalRevenueClient;
            _logger = logger;
        }

        public async Task<Result<decimal>> GetTotalRevenueAsync(CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("AuditService: Sending GetTotalRevenueRequest");
                
                var response = await _getTotalRevenueClient.GetResponse<GetTotalRevenueResponse>(
                    new GetTotalRevenueRequest(),
                    ct);

                _logger.LogInformation("AuditService: Received response with total revenue: {TotalRevenue}", 
                    response.Message.TotalRevenue);

                return Result<decimal>.Success(response.Message.TotalRevenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return Result<decimal>.Failure("An error occurred while getting the total revenue");
            }
        }
    }
} 