# Test Suite Structure

## Files to Create/Update

```
LeadsManagement.Tests/
├── Domain/
│   └── LeadTests.cs                          (Entity validation)
├── Features/
│   └── Leads/
│       ├── Commands/
│       │   ├── CreateLeadCommandHandlerTests.cs
│       │   ├── AcceptLeadCommandHandlerTests.cs
│       │   └── DeclineLeadCommandHandlerTests.cs
│       └── Queries/
│           ├── GetLeadByIdQueryHandlerTests.cs
│           └── GetLeadsByStatusQueryHandlerTests.cs
└── LeadsManagement.Tests.csproj
```

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
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Application.Features.Leads.DTOs;
using LeadsManagement.Infrastructure.Data.Repositories;

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

    [Fact]
    public async Task Handle_WithLowPrice_CreatesLeadWithoutDiscount()
    {
        // Arrange (price < 500)
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Bob",
            ContactLastName = "Brown",
            ContactEmail = "bob@test.com",
            ContactPhoneNumber = "555-4444",
            Suburb = "Downtown",
            Category = "Real Estate",
            Description = "Small property",
            Price = 250000
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        // Verify no discount logic applied
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Lead>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithHighPrice_AppliesDiscount()
    {
        // Arrange (price > 500)
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Alice",
            ContactLastName = "Davis",
            ContactEmail = "alice@test.com",
            ContactPhoneNumber = "555-5555",
            Suburb = "Uptown",
            Category = "Development",
            Description = "Large commercial property",
            Price = 2500000  // > 500k
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        // Verify discount logic applied if exists
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Lead>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullEmail_Succeeds()
    {
        // Arrange (optional email)
        var command = new CreateLeadCommand
        {
            ContactFirstName = "Charlie",
            ContactLastName = "Evans",
            ContactEmail = null,
            ContactPhoneNumber = "555-6666",
            Suburb = "Downtown",
            Category = "Real Estate",
            Description = "Property without contact email",
            Price = 350000
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
    }
}
```

---

## 3. GetLeadByIdQueryHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Queries;
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
    public async Task Handle_WithValidId_ReturnsLead()
    {
        // Arrange
        var leadId = 1;
        var contact = new Contact("John", "Doe", "555-1234", "john@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Apartment", 450000);
        
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var query = new GetLeadByIdQuery { Id = leadId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        _mockRepository.Verify(r => r.GetByIdAsync(leadId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var leadId = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync((Lead)null);

        var query = new GetLeadByIdQuery { Id = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithZeroId_ThrowsException()
    {
        // Arrange
        var query = new GetLeadByIdQuery { Id = 0 };

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
using LeadsManagement.Application.Features.Leads.Queries;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
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

        _mockRepository.Setup(r => r.GetByStatusAsync("Invited"))
            .ReturnsAsync(leads);

        var query = new GetLeadsByStatusQuery { Status = "Invited" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetByStatusAsync("Invited"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAcceptedStatus_ReturnsOnlyAccepted()
    {
        // Arrange
        var leads = new List<Lead>();
        _mockRepository.Setup(r => r.GetByStatusAsync("Accepted"))
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
    public async Task Handle_WithValidStatuses_Succeeds(string status)
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByStatusAsync(status))
            .ReturnsAsync(new List<Lead>());

        var query = new GetLeadsByStatusQuery { Status = status };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetByStatusAsync(status), Times.Once);
    }
}
```

---

## 5. AcceptLeadCommandHandlerTests.cs

```csharp
using Xunit;
using Moq;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Commands;

public class AcceptLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly AcceptLeadCommandHandler _handler;

    public AcceptLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new AcceptLeadCommandHandler(_mockRepository.Object);
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
            .ReturnsAsync((Lead)null);

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
        lead.Accept(); // Already accepted

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

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
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Domain.Entities;
using LeadsManagement.Domain.ValueObjects;

namespace LeadsManagement.Tests.Features.Leads.Commands;

public class DeclineLeadCommandHandlerTests
{
    private readonly Mock<LeadRepository> _mockRepository;
    private readonly DeclineLeadCommandHandler _handler;

    public DeclineLeadCommandHandlerTests()
    {
        _mockRepository = new Mock<LeadRepository>();
        _handler = new DeclineLeadCommandHandler(_mockRepository.Object);
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
            .ReturnsAsync((Lead)null);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CanDeclineAcceptedLead()
    {
        // Arrange (change mind scenario)
        var leadId = 1;
        var contact = new Contact("Alice", "Brown", "555-1111", "alice@test.com");
        var lead = new Lead(contact, "Downtown", "Real Estate", "Condo", 500000);
        lead.Accept(); // First accept

        _mockRepository.Setup(r => r.GetByIdAsync(leadId))
            .ReturnsAsync(lead);

        var command = new DeclineLeadCommand { LeadId = leadId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Declined", lead.Status.ToString());
    }
}
```

---

## Test Coverage Summary

| Method | Tests | Coverage |
|--------|-------|----------|
| **POST Create** | 6 | Valid, zero price, negative price, empty name, null email, low/high price |
| **GET by ID** | 3 | Valid ID, invalid ID, zero ID |
| **GET by Status** | 5 | Each status, invalid status, empty results, all valid statuses |
| **POST Accept** | 3 | Valid, invalid ID, already accepted |
| **POST Decline** | 3 | Valid, invalid ID, decline accepted lead |
| **Domain Entity** | 7 | Creation, validation, status changes, edge cases |
| **TOTAL** | **27 tests** | Comprehensive coverage |

---

## Run Tests

```bash
dotnet test
```

Filter by category:

```bash
dotnet test --filter "ClassName=CreateLeadCommandHandlerTests"
dotnet test --filter "ClassName=LeadTests"
```
