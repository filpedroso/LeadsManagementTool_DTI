# Corrected Test Suite - Matching Actual Implementation

## Key Fixes
- `GetLeadByIdQuery` uses `LeadId` (not `Id`)
- Query handlers return `LeadDto` (not domain entities)
- Repository method is `GetLeadsByStatusAsync` (takes `LeadStatus` enum, not string)
- Handler constructors need `IEmailService` parameter
- Namespace is `LeadsManagement.Application.Leads` (not `Features.Leads`)

---

## 1. LeadTests.cs (Domain Entity Validation)

```csharp
using Xunit;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Domain;

public class LeadTests
{
    [Fact]
    public void CreateLead_WithValidData_Succeeds()
    {
        // Arrange
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        
        // Act
        var lead = new Lead(contact, "Downtown", "Real Estate", "2-bed apartment", 450000);
        
        // Assert
        Assert.NotNull(lead);
        Assert.Equal("John", lead.Contact.FirstName);
        Assert.Equal(450000, lead.Price.Amount);
        Assert.Equal("Invited", lead.Status.ToString());
    }

    [Fact]
    public void CreateLead_WithPriceZero_ThrowsException()
    {
        // Arrange
        var contact = new Contact("Jane", "Smith", "555-5678", "jane@test.com");
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Lead(contact, "Uptown", "Commercial", "Office space", 0));
    }

    [Fact]
    public void CreateLead_WithNegativePrice_ThrowsException()
    {
        // Arrange
        var contact = new Contact("Bob", "Wilson", "555-9999", "bob@test.com");
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Lead(contact, "Midtown", "Development", "Land parcel", -100000));
    }

    [Fact]
    public void CreateLead_WithEmptyFirstName_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Contact("", "Doe", "555-1234", "test@test.com"));
    }

    [Fact]
    public void CreateLead_WithNullEmail_Succeeds()
    {
        // Arrange & Act
        var contact = new Contact("Alice", "Brown", "555-1111", null);
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 300000);
        
        // Assert
        Assert.NotNull(lead);
        Assert.Null(lead.Contact.Email);
    }

    [Fact]
    public void Lead_AcceptStatus_UpdatesCorrectly()
    {
        // Arrange
        var contact = new Contact("Charlie", "Davis", "555-2222", "charlie@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "House", 500000);
        
        // Act
        lead.Accept();
        
        // Assert
        Assert.Equal("Accepted", lead.Status.ToString());
    }

    [Fact]
    public void Lead_DeclineStatus_UpdatesCorrectly()
    {
        // Arrange
        var contact = new Contact("Diana", "Evans", "555-3333", "diana@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Condo", 600000);
        
        // Act
        lead.Decline();
        
        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
    }
}
```

---

## 2. CreateLeadCommandHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Commands;

public class CreateLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly CreateLeadCommandHandler _handler;

    public CreateLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new CreateLeadCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_ReturnsLeadId()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "John",
            ContactLastName = "Doe",
            ContactEmail = "john@test.com",
            ContactPhoneNumber = "555-1234",
            Suburb = "Downtown",
            Category = "Real Estate",
            Description = "2-bedroom apartment",
            Price = 450000
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Lead>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Lead>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithPriceZero_ThrowsException()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Jane",
            ContactLastName = "Smith",
            ContactEmail = "jane@test.com",
            ContactPhoneNumber = "555-5678",
            Suburb = "Uptown",
            Category = "Commercial",
            Description = "Office space",
            Price = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyFirstName_ThrowsException()
    {
        // Arrange
        var command = new CreateLeadCommand
        {
            ContactFirstName = "",
            ContactLastName = "Wilson",
            ContactEmail = "test@test.com",
            ContactPhoneNumber = "555-9999",
            Suburb = "Midtown",
            Category = "Development",
            Description = "Land parcel",
            Price = 750000
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}
```

---

## 3. GetLeadByIdQueryHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Leads.Queries;
using LeadsManagement.Application.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Queries;

public class GetLeadByIdQueryHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly GetLeadByIdQueryHandler _handler;

    public GetLeadByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new GetLeadByIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ReturnsLeadDto()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 450000);
        
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var query = new GetLeadByIdQuery { LeadId = leadId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LeadDto>(result);
        _mockRepository.Verify(r => r.GetByIdAsync(leadId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var query = new GetLeadByIdQuery { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithZeroId_ThrowsException()
    {
        // Arrange
        var query = new GetLeadByIdQuery { LeadId = 0 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}
```

---

## 4. GetLeadsByStatusQueryHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Leads.Queries;
using LeadsManagement.Application.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.Enums;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Queries;

public class GetLeadsByStatusQueryHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly GetLeadsByStatusQueryHandler _handler;

    public GetLeadsByStatusQueryHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new GetLeadsByStatusQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithInvitedStatus_ReturnsLeads()
    {
        // Arrange
        var leads = new List<Lead>
        {
            new Lead(
                new Contact("John", "Doe", "555-1234", "john@test.com"),
                "Downtown", "Real Estate", "Apartment", 450000),
            new Lead(
                new Contact("Jane", "Smith", "555-5678", "jane@test.com"),
                "Uptown", "Commercial", "Office", 600000)
        };

        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Invited))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Invited" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(r => r.GetLeadsByStatusAsync(LeadStatus.Invited), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAcceptedStatus_ReturnsOnlyAccepted()
    {
        // Arrange
        var leads = new List<Lead>();
        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(LeadStatus.Accepted))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Accepted" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ThrowsException()
    {
        // Arrange
        var query = new GetLeadsByStatusQuery { Status = "InvalidStatus" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Theory]
    [InlineData("Invited")]
    [InlineData("Accepted")]
    [InlineData("Declined")]
    public async Task Handle_WithValidStatuses_Succeeds(string statusString)
    {
        // Arrange
        var status = Enum.Parse<LeadStatus>(statusString);
        _mockRepository.Setup(r => r.GetLeadsByStatusAsync(status))
            .ReturnsAsync(new List<Lead>());

        var query = new GetLeadsByStatusQuery { Status = statusString };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetLeadsByStatusAsync(status), Times.Once);
    }
}
```

---

## 5. AcceptLeadCommandHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Commands;

public class AcceptLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly AcceptLeadCommandHandler _handler;

    public AcceptLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new AcceptLeadCommandHandler(_mockRepository.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_AcceptsAndSaves()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 450000);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Accepted", lead.Status.ToString());
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ThrowsException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithAlreadyAcceptedLead_UpdatesStatus()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Jane", "Smith", "555-5678", "jane@test.com");
        var lead = new Lead(contact, "Uptown", "Commercial", "Office", 600000);
        lead.Accept();

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new AcceptLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Accepted", lead.Status.ToString());
    }
}
```

---

## 6. DeclineLeadCommandHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Commands;

public class DeclineLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly DeclineLeadCommandHandler _handler;

    public DeclineLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _handler = new DeclineLeadCommandHandler(_mockRepository.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_WithValidLead_DeclinesAndSaves()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Bob", "Wilson", "555-9999", "bob@test.com");
        var lead = new Lead(contact, "Midtown", "Development", "Land", 750000);

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLeadId_ThrowsException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead?)null);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CanDeclineAcceptedLead()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("Alice", "Brown", "555-1111", "alice@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Condo", 500000);
        lead.Accept();

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
    }
}
```

---

## Summary of Changes

| Issue | Fix |
|-------|-----|
| `GetLeadByIdQuery` property | Changed `Id` → `LeadId` |
| Query return type | Changed to expect `LeadDto` instead of domain Lead |
| Repository method | Changed `GetByStatusAsync(string)` → `GetLeadsByStatusAsync(LeadStatus enum)` |
| Handler constructors | Added `IEmailService` parameter for Accept/Decline handlers |
| Namespace paths | Updated from `Features.Leads` → `Leads` |
| Mock setup | Added `.Returns(Task.CompletedTask)` for async void methods |

Replace all test files with these corrected versions and tests should compile! 🎯
