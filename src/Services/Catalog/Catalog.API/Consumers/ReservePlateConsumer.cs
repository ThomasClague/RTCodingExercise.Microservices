using Catalog.API.Contracts.Services;
using Contracts.Plates;
using MassTransit;

namespace Catalog.API.Consumers
{
    public class ReservePlateConsumer : IConsumer<ReservePlateRequest>
    {
        private readonly IPlatesService _platesService;
        private readonly ILogger<ReservePlateConsumer> _logger;

        public ReservePlateConsumer(
            IPlatesService platesService,
            ILogger<ReservePlateConsumer> logger)
        {
            _platesService = platesService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReservePlateRequest> context)
        {
            var result = await _platesService.ReservePlateAsync(context.Message.PlateId, context.CancellationToken);

            if (result.IsSuccess)
            {
                var response = new ReservePlateResponse
                {
                    Plate = PlateDTO.FromPlate(result.Value)
                };

                await context.RespondAsync(response);
            }
            else
            {
                _logger.LogWarning("Plate reservation failed: {Errors}", string.Join(", ", result.Errors));
                await context.RespondAsync(
                    new ReservePlateFailureResponse(result.Errors));
            }
        }
    }
} 