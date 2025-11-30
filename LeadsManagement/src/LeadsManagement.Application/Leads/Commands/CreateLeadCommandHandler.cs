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
    private readonly ILeadRepository _leadRepository;

    public CreateLeadCommandHandler(ILeadRepository leadRepository)
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
