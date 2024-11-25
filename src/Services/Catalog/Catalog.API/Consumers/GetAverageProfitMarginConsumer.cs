using Catalog.API.Contracts.Services;
using Contracts.Plates;
using MassTransit;

namespace Catalog.API.Consumers
{
    public class GetAverageProfitMarginConsumer : IConsumer<GetAverageProfitMarginRequest>
    {
        private readonly IPlatesService _platesService;
        private readonly ILogger<GetAverageProfitMarginConsumer> _logger;

        public GetAverageProfitMarginConsumer(
            IPlatesService platesService,
            ILogger<GetAverageProfitMarginConsumer> logger)
        {
            _platesService = platesService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetAverageProfitMarginRequest> context)
        {
            _logger.LogInformation("Getting average profit margin");
            
            var averageProfitMargin = await _platesService.GetAverageProfitMarginAsync(context.CancellationToken);
            
            await context.RespondAsync(new GetAverageProfitMarginResponse
            {
                AverageProfitMargin = averageProfitMargin
            });
        }
    }
} 