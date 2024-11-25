using Contracts.Audit;
using MassTransit;
using Audit.API.Services;

namespace Audit.API.Consumers
{
    public class GetTotalRevenueConsumer : IConsumer<GetTotalRevenueRequest>
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<GetTotalRevenueConsumer> _logger;

        public GetTotalRevenueConsumer(
            IAuditService auditService,
            ILogger<GetTotalRevenueConsumer> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetTotalRevenueRequest> context)
        {
            _logger.LogInformation("GetTotalRevenueConsumer: Starting to process request");
            
            var totalRevenue = await _auditService.GetTotalRevenueAsync(context.CancellationToken);
            
            _logger.LogInformation("GetTotalRevenueConsumer: Total revenue calculated: {TotalRevenue}", totalRevenue);
            
            await context.RespondAsync(new GetTotalRevenueResponse
            {
                TotalRevenue = totalRevenue
            });
        }
    }
} 