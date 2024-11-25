using Catalog.API.Contracts.Services;
using Contracts.Pagination;
using Contracts.Plates;
using MassTransit;
using MassTransit.Initializers;

namespace Catalog.API.Consumers
{
    public class GetPlatesConsumer : IConsumer<GetPlatesRequest>
    {
        private readonly IPlatesService _platesService;

        public GetPlatesConsumer(IPlatesService platesService)
        {
            _platesService = platesService;
        }

        public async Task Consume(ConsumeContext<GetPlatesRequest> context)
        {
            var paginatedPlates = await _platesService.GetPlatesAsync(
                context.Message.PlatesFilter, 
                context.Message.IsAdmin,
                context.CancellationToken);

            var paginatedPlatesDto = new PagedResponse<PlateDTO>(
                paginatedPlates.Data.Select(plate => PlateDTO.FromPlate(plate)).ToList(),
                paginatedPlates.Pagination);

            var response = new GetPlatesResponse
            {
                PagedResponse = paginatedPlatesDto
            };

            await context.RespondAsync(response);
        }
    }
}
