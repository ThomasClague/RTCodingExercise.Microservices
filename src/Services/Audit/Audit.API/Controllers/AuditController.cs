using Audit.API.Models.DTOs;
using Audit.API.Models.Filters;
using Audit.API.Services;
using Contracts.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Audit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditController> _logger;

        public AuditController(
            IAuditService auditService,
            ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paged list of audit records based on the provided filters
        /// </summary>
        /// <param name="filter">Filter criteria for audit records</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A paged list of audit records</returns>
        /// <response code="200">Returns the paged list of audit records</response>
        /// <response code="400">If the filter parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<AuditRecordDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PagedResponse<AuditRecordDto>>> GetAuditRecords(
            [FromQuery] AuditRecordFilter filter, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting audit records with filter: {@Filter}", filter);
            var result = await _auditService.GetAuditRecordsAsync(filter, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific audit record by its ID
        /// </summary>
        /// <param name="id">The ID of the audit record to retrieve</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested audit record</returns>
        /// <response code="200">Returns the requested audit record</response>
        /// <response code="404">If the audit record was not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AuditRecordDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<AuditRecordDto>> GetAuditRecordById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting audit record with ID: {Id}", id);
            var record = await _auditService.GetAuditRecordByIdAsync(id, cancellationToken);

            if (record == null)
            {
                _logger.LogWarning("Audit record with ID {Id} not found", id);
                return NotFound();
            }

            return Ok(record);
        }
    }
} 