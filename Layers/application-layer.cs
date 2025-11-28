// ===================================================================
// FILE: src/LeadsManagement.Application/Common/Models/Result.cs
// ===================================================================

namespace LeadsManagement.Application.Common.Models;

/// <summary>
/// Padrão Result para retornar dados de forma padronizada
/// Encapsula sucesso, erros e dados
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error, Errors = new() { error } };
    public static Result<T> Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error, Errors = new() { error } };
    public static Result Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Common/Models/ApiException.cs
// ===================================================================

namespace LeadsManagement.Application.Common.Models;

/// <summary>
/// Exceção customizada para erros da aplicação
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new();

    public ApiException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
        Errors.Add(message);
    }

    public ApiException(List<string> errors, int statusCode = 500) : base(string.Join(", ", errors))
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/DTOs/LeadDto.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// DTO para representar um Lead na API
/// Usado para retornar dados de lead
/// </summary>
public class LeadDto
{
    public int Id { get; set; }
    public string ContactFirstName { get; set; }
    public string? ContactLastName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public DateTime DateCreated { get; set; }
    public string Suburb { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/DTOs/CreateLeadDto.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// DTO para criar um novo Lead
/// Recebido do frontend
/// </summary>
public class CreateLeadDto
{
    public string ContactFirstName { get; set; }
    public string? ContactLastName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string Suburb { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Commands/CreateLeadCommand.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

/// <summary>
/// Command para criar um novo Lead
/// Implementa padrão CQRS - Command
/// </summary>
public class CreateLeadCommand : IRequest<int>
{
    public string ContactFirstName { get; set; }
    public string? ContactLastName { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string Suburb { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Commands/CreateLeadCommandHandler.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;
using LeadsManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Handler que processa CreateLeadCommand
/// Cria novo lead no banco de dados
/// </summary>
public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, int>
{
    private readonly LeadRepository _leadRepository;

    public CreateLeadCommandHandler(LeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<int> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        // Validar se preço é válido
        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than zero");

        // Criar Value Objects
        var contact = new Contact(
            firstName: request.ContactFirstName,
            lastName: request.ContactLastName,
            phoneNumber: request.ContactPhoneNumber,
            email: request.ContactEmail);

        // Criar entidade Lead
        var lead = new Lead(
            contact: contact,
            suburb: request.Suburb,
            category: request.Category,
            description: request.Description,
            price: request.Price);

        // Salvar no repositório
        await _leadRepository.AddAsync(lead);
        await _leadRepository.SaveChangesAsync();

        return lead.Id;
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Commands/AcceptLeadCommand.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

/// <summary>
/// Command para aceitar um Lead
/// </summary>
public class AcceptLeadCommand : IRequest<Unit>
{
    public int LeadId { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Commands/AcceptLeadCommandHandler.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Infrastructure.Services;

/// <summary>
/// Handler que processa AcceptLeadCommand
/// Aceita um lead, aplica desconto se aplicável
/// E envia notificação por email
/// </summary>
public class AcceptLeadCommandHandler : IRequestHandler<AcceptLeadCommand, Unit>
{
    private readonly LeadRepository _leadRepository;
    private readonly IEmailService _emailService;

    public AcceptLeadCommandHandler(LeadRepository leadRepository, IEmailService emailService)
    {
        _leadRepository = leadRepository;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(AcceptLeadCommand request, CancellationToken cancellationToken)
    {
        // Buscar lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        // Aceitar lead (aplica lógica de desconto)
        var priceBeforeDiscount = lead.Price.Amount;
        lead.Accept();
        var priceAfterDiscount = lead.Price.Amount;
        var discountApplied = priceAfterDiscount < priceBeforeDiscount;

        // Salvar mudanças
        await _leadRepository.UpdateAsync(lead);
        await _leadRepository.SaveChangesAsync();

        // Enviar notificação de email
        var emailAddress = lead.Contact.Email ?? "vendas@test.com";
        await _emailService.SendLeadAcceptedNotificationAsync(
            leadId: lead.Id,
            contactEmail: emailAddress,
            finalPrice: priceAfterDiscount,
            discountApplied: discountApplied);

        return Unit.Value;
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Commands/DeclineLeadCommand.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;

/// <summary>
/// Command para recusar um Lead
/// </summary>
public class DeclineLeadCommand : IRequest<Unit>
{
    public int LeadId { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Commands/DeclineLeadCommandHandler.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Commands;

using MediatR;
using LeadsManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Handler que processa DeclineLeadCommand
/// Recusa um lead
/// </summary>
public class DeclineLeadCommandHandler : IRequestHandler<DeclineLeadCommand, Unit>
{
    private readonly LeadRepository _leadRepository;

    public DeclineLeadCommandHandler(LeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<Unit> Handle(DeclineLeadCommand request, CancellationToken cancellationToken)
    {
        // Buscar lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        // Recusar lead
        lead.Decline();

        // Salvar mudanças
        await _leadRepository.UpdateAsync(lead);
        await _leadRepository.SaveChangesAsync();

        return Unit.Value;
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Queries/GetLeadsByStatusQuery.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// Query para buscar leads por status
/// Implementa padrão CQRS - Query
/// </summary>
public class GetLeadsByStatusQuery : IRequest<List<LeadDto>>
{
    public string Status { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Queries/GetLeadsByStatusQueryHandler.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Infrastructure.Data.Repositories;
using Mapster;

/// <summary>
/// Handler que processa GetLeadsByStatusQuery
/// Busca todos os leads com um status específico
/// </summary>
public class GetLeadsByStatusQueryHandler : IRequestHandler<GetLeadsByStatusQuery, List<LeadDto>>
{
    private readonly LeadRepository _leadRepository;

    public GetLeadsByStatusQueryHandler(LeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<List<LeadDto>> Handle(GetLeadsByStatusQuery request, CancellationToken cancellationToken)
    {
        // Parsear status
        if (!Enum.TryParse<LeadStatus>(request.Status, true, out var status))
            throw new ArgumentException($"Invalid status: {request.Status}");

        // Buscar leads
        var leads = await _leadRepository.GetLeadsByStatusAsync(status);

        // Mapear para DTOs
        var dtos = leads
            .Adapt<List<LeadDto>>()
            .ToList();

        return dtos;
    }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Queries/GetLeadByIdQuery.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;

/// <summary>
/// Query para buscar um lead específico por ID
/// </summary>
public class GetLeadByIdQuery : IRequest<LeadDto>
{
    public int LeadId { get; set; }
}

// ===================================================================
// FILE: src/LeadsManagement.Application/Features/Leads/Queries/GetLeadByIdQueryHandler.cs
// ===================================================================

namespace LeadsManagement.Application.Features.Leads.Queries;

using MediatR;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using Mapster;

/// <summary>
/// Handler que processa GetLeadByIdQuery
/// Busca um lead específico pelo ID
/// </summary>
public class GetLeadByIdQueryHandler : IRequestHandler<GetLeadByIdQuery, LeadDto>
{
    private readonly LeadRepository _leadRepository;

    public GetLeadByIdQueryHandler(LeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<LeadDto> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        // Buscar lead
        var lead = await _leadRepository.GetByIdAsync(request.LeadId);
        if (lead == null)
            throw new InvalidOperationException($"Lead with id {request.LeadId} not found");

        // Mapear para DTO
        return lead.Adapt<LeadDto>();
    }
}
