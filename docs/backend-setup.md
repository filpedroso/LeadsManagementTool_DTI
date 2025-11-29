# 🚀 BACKEND - Setup Completo

## Estrutura de Pastas

```
LeadsManagementAPI/
├── src/
│   ├── LeadsManagement.API/                    # Presentation Layer (ASP.NET Core)
│   │   ├── Controllers/
│   │   │   ├── LeadsController.cs
│   │   ├── Middleware/
│   │   │   └── ErrorHandlingMiddleware.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── LeadsManagement.API.csproj
│   │
│   ├── LeadsManagement.Application/            # Application Layer (Features)
│   │   ├── Common/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IEmailService.cs
│   │   │   │   └── IRepository.cs
│   │   │   ├── Models/
│   │   │   │   ├── Result.cs
│   │   │   │   └── ApiException.cs
│   │   ├── Features/
│   │   │   ├── Leads/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── AcceptLeadCommand.cs
│   │   │   │   │   ├── AcceptLeadCommandHandler.cs
│   │   │   │   │   ├── DeclineLeadCommand.cs
│   │   │   │   │   ├── DeclineLeadCommandHandler.cs
│   │   │   │   │   ├── CreateLeadCommand.cs
│   │   │   │   │   └── CreateLeadCommandHandler.cs
│   │   │   │   ├── Queries/
│   │   │   │   │   ├── GetLeadsByStatusQuery.cs
│   │   │   │   │   ├── GetLeadsByStatusQueryHandler.cs
│   │   │   │   │   ├── GetLeadByIdQuery.cs
│   │   │   │   │   └── GetLeadByIdQueryHandler.cs
│   │   │   │   ├── DTOs/
│   │   │   │   │   ├── LeadDto.cs
│   │   │   │   │   ├── CreateLeadDto.cs
│   │   │   │   │   └── LeadResponseDto.cs
│   │   │   │   ├── Validators/
│   │   │   │   │   ├── AcceptLeadCommandValidator.cs
│   │   │   │   │   └── CreateLeadCommandValidator.cs
│   │   ├── LeadsManagement.Application.csproj
│   │
│   ├── LeadsManagement.Domain/                 # Domain Layer (Entities & Business Rules)
│   │   ├── Entities/                           # Innermost Layer, agnostic to DB, API or UI types
│   │   │   └── Lead.cs
│   │   ├── Enums/
│   │   │   └── LeadStatus.cs
│   │   ├── ValueObjects/
│   │   │   ├── Money.cs
│   │   │   └── Contact.cs
│   │   │
│   │   └── LeadsManagement.Domain.csproj
│   │
│   └── LeadsManagement.Infrastructure/         # Infrastructure Layer
│       ├── Data/
│       │   ├── Contexts/
│       │   │   └── ApplicationDbContext.cs
│       │   ├── Migrations/
│       │   │   ├── 20251127000000_InitialCreate.cs
│       │   │   ├── 20251127000000_InitialCreate.Designer.cs
│       │   │   └── ApplicationDbContextModelSnapshot.cs
│       │   ├── Repositories/
│       │   │   ├── LeadRepository.cs
│       │   │   └── RepositoryBase.cs
│       │   └── Configurations/
│       │       └── LeadConfiguration.cs
│       ├── Services/
│       │   ├── EmailService.cs
│       │   └── LoggingService.cs
│       ├── Extensions/
│       │   └── InfrastructureServiceCollectionExtensions.cs
│       └── LeadsManagement.Infrastructure.csproj
│
├── tests/
│   └── LeadsManagement.Tests/
│       ├── Features/
│       │   └── Leads/
│       │       ├── AcceptLeadCommandHandlerTests.cs
│       │       ├── DeclineLeadCommandHandlerTests.cs
│       │       └── GetLeadsByStatusQueryHandlerTests.cs
│       ├── Domain/
│       │   └── LeadTests.cs
│       └── LeadsManagement.Tests.csproj
│
├── LeadsManagement.sln
└── README.md
```

## Comandos para Criar Projetos

```bash
# Criar solução
dotnet new sln -n LeadsManagement

# Criar projetos
dotnet new webapi -n LeadsManagement.API -o src/LeadsManagement.API
dotnet new classlib -n LeadsManagement.Application -o src/LeadsManagement.Application
dotnet new classlib -n LeadsManagement.Domain -o src/LeadsManagement.Domain
dotnet new classlib -n LeadsManagement.Infrastructure -o src/LeadsManagement.Infrastructure
dotnet new xunit -n LeadsManagement.Tests -o tests/LeadsManagement.Tests

# Adicionar à solução
dotnet sln add src/LeadsManagement.API/LeadsManagement.API.csproj
dotnet sln add src/LeadsManagement.Application/LeadsManagement.Application.csproj
dotnet sln add src/LeadsManagement.Domain/LeadsManagement.Domain.csproj
dotnet sln add src/LeadsManagement.Infrastructure/LeadsManagement.Infrastructure.csproj
dotnet sln add tests/LeadsManagement.Tests/LeadsManagement.Tests.csproj

# Adicionar referências entre projetos
cd src/LeadsManagement.API
dotnet add reference ../LeadsManagement.Application/LeadsManagement.Application.csproj
dotnet add reference ../LeadsManagement.Infrastructure/LeadsManagement.Infrastructure.csproj

cd ../LeadsManagement.Application
dotnet add reference ../LeadsManagement.Domain/LeadsManagement.Domain.csproj

cd ../LeadsManagement.Infrastructure
dotnet add reference ../LeadsManagement.Domain/LeadsManagement.Domain.csproj
dotnet add reference ../LeadsManagement.Application/LeadsManagement.Application.csproj

cd ../../tests/LeadsManagement.Tests
dotnet add reference ../../src/LeadsManagement.API/LeadsManagement.API.csproj
dotnet add reference ../../src/LeadsManagement.Application/LeadsManagement.Application.csproj
dotnet add reference ../../src/LeadsManagement.Domain/LeadsManagement.Domain.csproj
dotnet add reference ../../src/LeadsManagement.Infrastructure/LeadsManagement.Infrastructure.csproj
```

## NuGet Packages Necessários

```bash
# Na pasta API
dotnet add package MediatR
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
dotnet add package Mapster
dotnet add package FluentValidation
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design

# Na pasta Application
dotnet add package MediatR
dotnet add package FluentValidation

# Na pasta Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.Extensions.Logging
dotnet add package Serilog
dotnet add package Serilog.Extensions.Logging

# Na pasta Tests
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package FluentAssertions
```
