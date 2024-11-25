using Catalog.API.Contracts.Services;
using Contracts.Pagination;
using Contracts.Plates;

namespace Catalog.API.Controllers
{
    [Route("Plates")]
    [ApiController]
    public class PlatesController : ControllerBase
    {
        private readonly IPlatesService _platesService;

        public PlatesController(IPlatesService platesService)
        {
            _platesService = platesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Plate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPlates(
            [FromQuery] PlatesFilter filter, 
            bool isAdmin,
            CancellationToken cancellationToken)
        {
            return Ok(await _platesService.GetPlatesAsync(filter, isAdmin, cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Plate), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlate(
            [FromBody] CreatePlateCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _platesService.CreatePlateAsync(command, cancellationToken);
            
            if (result.IsFailure)
            {
                return result.Errors.Count == 1 
                    ? BadRequest(result.Errors[0])
                    : BadRequest(result.Errors);
            }

            return CreatedAtAction(
                nameof(GetPlateById), 
                new { id = result.Value.Id }, 
                result.Value);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Plate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlateById(Guid id)
        {
            // TODO: Implement this endpoint
            return Ok();
        }
    }
}
