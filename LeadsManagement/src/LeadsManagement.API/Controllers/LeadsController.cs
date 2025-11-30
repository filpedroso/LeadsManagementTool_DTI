namespace LeadsManagement.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Application.Features.Leads.DTOs;

// API controller that exposes REST endpoints for CRUD operations
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class LeadsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(IMediator mediator, ILogger<LeadsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    // GET: api/v1/leads/status/{status}
    // Searches Leads by status (Invited, Accepted, Declined)
    // returning a list of Leads with {status}
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<LeadDto>>> GetLeadsByStatus(string status)
    {
        try
        {
            _logger.LogInformation($"Fetching leads with status: {status}");

            var query = new GetLeadsByStatusQuery { Status = status };
            var leads = await _mediator.Send(query);

            _logger.LogInformation($"Successfully fetched {leads.Count} leads with status: {status}");
            return Ok(leads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching leads with status: {status}");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Searches a Lead by its ID
    [HttpGet("{id}")]
    public async Task<ActionResult<LeadDto>> GetLeadById(int id)
    {
        try
        {
            _logger.LogInformation($"Fetching lead with ID: {id}");

            var query = new GetLeadByIdQuery { LeadId = id };
            var lead = await _mediator.Send(query);

            _logger.LogInformation($"Successfully fetched lead with ID: {id}");
            return Ok(lead);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Lead not found with ID: {id}");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching lead with ID: {id}");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Creates a new Lead for testing purposes
    // Accessed only by Swagger UI (not by the Frontend)
    [HttpPost]
    public async Task<ActionResult<int>> CreateLead([FromBody] CreateLeadDto dto)
    {
        try
        {
            _logger.LogInformation($"Creating new lead for contact: {dto.ContactFirstName}");

            var command = new CreateLeadCommand
            {
                ContactFirstName = dto.ContactFirstName,
                ContactLastName = dto.ContactLastName,
                ContactEmail = dto.ContactEmail,
                ContactPhoneNumber = dto.ContactPhoneNumber,
                Suburb = dto.Suburb,
                Category = dto.Category,
                Description = dto.Description,
                Price = dto.Price
            };

            var leadId = await _mediator.Send(command);

            _logger.LogInformation($"Successfully created lead with ID: {leadId}");
            return CreatedAtAction(nameof(GetLeadById), new { id = leadId }, new { id = leadId });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid lead data");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lead");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Accepts a Lead, returning a confirmation message
    [HttpPost("{id}/accept")]
    public async Task<ActionResult> AcceptLead(int id)
    {
        try
        {
            _logger.LogInformation($"Accepting lead with ID: {id}");

            var command = new AcceptLeadCommand { LeadId = id };
            await _mediator.Send(command);

            _logger.LogInformation($"Successfully accepted lead with ID: {id}");
            return Ok(new { message = $"Lead {id} accepted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Error accepting lead with ID: {id}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error accepting lead with ID: {id}");
            return BadRequest(new { error = ex.Message });
        }
    }

    // Declines a Lead, returning a confirmation message
    [HttpPost("{id}/decline")]
    public async Task<ActionResult> DeclineLead(int id)
    {
        try
        {
            _logger.LogInformation($"Declining lead with ID: {id}");

            var command = new DeclineLeadCommand { LeadId = id };
            await _mediator.Send(command);

            _logger.LogInformation($"Successfully declined lead with ID: {id}");
            return Ok(new { message = $"Lead {id} declined successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Error declining lead with ID: {id}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error declining lead with ID: {id}");
            return BadRequest(new { error = ex.Message });
        }
    }
}
