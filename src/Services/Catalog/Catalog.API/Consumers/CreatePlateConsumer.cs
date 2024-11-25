using Catalog.API.Contracts.Services;
using Contracts.Plates;
using MassTransit;

namespace Catalog.API.Consumers
{
    public class CreatePlateConsumer : IConsumer<CreatePlateRequest>
    {
        private readonly IPlatesService _platesService;
        private readonly ILogger<CreatePlateConsumer> _logger;

        public CreatePlateConsumer(
            IPlatesService platesService,
            ILogger<CreatePlateConsumer> logger)
        {
            _platesService = platesService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreatePlateRequest> context)
        {
            var result = await _platesService.CreatePlateAsync(context.Message.Command, context.CancellationToken);

            if (result.IsSuccess)
            {
                var response = new CreatePlateResponse
                {
                    Plate = PlateDTO.FromPlate(result.Value)
                };

                await context.RespondAsync(response);
            }
            else
            {
                _logger.LogWarning("Plate creation failed: {Errors}", string.Join(", ", result.Errors));
                await context.RespondAsync(
                    new CreatePlateFailureResponse(result.Errors));
            }
        }
    }
} 