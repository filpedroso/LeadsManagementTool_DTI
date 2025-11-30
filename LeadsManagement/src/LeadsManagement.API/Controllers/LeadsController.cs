namespace LeadsManagement.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// API Controller para gerenciar Leads
/// Expõe endpoints REST para operações CRUD
/// </summary>
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

    /// <summary>
    /// GET: api/v1/leads/status/{status}
    /// Busca todos os leads com um status específico
    /// </summary>
    /// <param name="status">Status do lead (Invited, Accepted, Declined)</param>
    /// <returns>Lista de leads com o status especificado</returns>
    [HttpGet("status/{status}")]
    // [ProduceResponseType(StatusCodes.Status200OK)]
    // [ProduceResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// GET: api/v1/leads/{id}
    /// Busca um lead específico por ID
    /// </summary>
    /// <param name="id">ID do lead</param>
    /// <returns>Dados do lead</returns>
    [HttpGet("{id}")]
    // [ProduceResponseType(StatusCodes.Status200OK)]
    // [ProduceResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// POST: api/v1/leads
    /// Cria um novo lead
    /// </summary>
    /// <param name="dto">Dados do novo lead</param>
    /// <returns>ID do lead criado</returns>
    [HttpPost]
    // [ProduceResponseType(StatusCodes.Status201Created)]
    // [ProduceResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// POST: api/v1/leads/{id}/accept
    /// Aceita um lead e aplica desconto se aplicável
    /// Envia notificação por email
    /// </summary>
    /// <param name="id">ID do lead</param>
    /// <returns>Confirmação de aceitação</returns>
    [HttpPost("{id}/accept")]
    // [ProduceResponseType(StatusCodes.Status200OK)]
    // [ProduceResponseType(StatusCodes.Status404NotFound)]
    // [ProduceResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// POST: api/v1/leads/{id}/decline
    /// Recusa um lead
    /// </summary>
    /// <param name="id">ID do lead</param>
    /// <returns>Confirmação de recusa</returns>
    [HttpPost("{id}/decline")]
    // [ProduceResponseType(StatusCodes.Status200OK)]
    // [ProduceResponseType(StatusCodes.Status404NotFound)]
    // [ProduceResponseType(StatusCodes.Status400BadRequest)]
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
