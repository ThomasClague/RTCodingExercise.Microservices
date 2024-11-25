using Catalog.API.Contracts.Services;
using Contracts.Plates;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Consumers
{
    public class BuyPlateConsumer : IConsumer<BuyPlateRequest>
    {
        private readonly IPlatesService _platesService;
        private readonly ILogger<BuyPlateConsumer> _logger;

        public BuyPlateConsumer(
            IPlatesService platesService,
            ILogger<BuyPlateConsumer> logger)
        {
            _platesService = platesService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BuyPlateRequest> context)
        {
            var result = await _platesService.BuyPlateAsync(
                context.Message.PlateId, 
                context.Message.DiscountCode,
                context.CancellationToken);

            if (result.IsSuccess)
            {
                var response = new BuyPlateResponse
                {
                    Plate = PlateDTO.FromPlate(result.Value)
                };

                await context.RespondAsync(response);
            }
            else
            {
                _logger.LogWarning("Plate purchase failed: {Errors}", string.Join(", ", result.Errors));
                await context.RespondAsync(
                    new BuyPlateFailureResponse(result.Errors));
            }
        }
    }
} 