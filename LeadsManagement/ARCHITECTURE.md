/*
# 🏗️ ARQUITETURA DO PROJETO

## Clean Architecture + Vertical Slices

Este projeto segue padrões modernos de desenvolvimento:

### Camadas

#### 1. **Domain Layer** (LeadsManagement.Domain)
- Entidades e Value Objects puros
- Lógica de negócio central
- Sem dependências externas
- Altamente testável

\`\`\`
Domain/
├── Entities/
│   └── Lead.cs (Agregado raiz)
├── ValueObjects/
│   ├── Money.cs
│   └── Contact.cs
├── Enums/
│   └── LeadStatus.cs
└── Events/
    ├── LeadAcceptedEvent.cs
    └── LeadDeclinedEvent.cs
\`\`\`

#### 2. **Application Layer** (LeadsManagement.Application)
- CQRS (Commands & Queries)
- DTOs (Data Transfer Objects)
- Handlers usando MediatR
- Lógica de aplicação

\`\`\`
Application/
├── Common/
│   ├── Models/
│   │   ├── Result.cs
│   │   └── ApiException.cs
│   └── Interfaces/
├── Features/
│   └── Leads/
│       ├── Commands/
│       ├── Queries/
│       ├── DTOs/
│       └── Validators/
\`\`\`

#### 3. **Infrastructure Layer** (LeadsManagement.Infrastructure)
- Entity Framework Core (ORM)
- Repositórios
- Serviços (Email, Logging)
- Configurações de BD

\`\`\`
Infrastructure/
├── Data/
│   ├── Contexts/ApplicationDbContext.cs
│   ├── Configurations/
│   ├── Migrations/
│   └── Repositories/
└── Services/
    └── EmailService.cs
\`\`\`

#### 4. **Presentation Layer** (LeadsManagement.API)
- Controllers ASP.NET Core
- Middleware
- Configurações de DI
- Program.cs

\`\`\`
API/
├── Controllers/LeadsController.cs
├── Middleware/ErrorHandlingMiddleware.cs
├── Extensions/ServiceCollectionExtensions.cs
├── Program.cs
└── appsettings.json
\`\`\`

### Padrões Utilizados

#### CQRS (Command Query Responsibility Segregation)
- Separa leitura de escrita
- Commands modificam estado
- Queries apenas consultam

#### Repository Pattern
- Abstração do acesso a dados
- Facilita testes e manutenção

#### Dependency Injection
- IoC Container nativo do ASP.NET Core
- Registrado em ServiceCollectionExtensions

#### Value Objects
- Imutáveis
- Encapsulam lógica de domínio
- Exemplo: Money, Contact

#### Domain Events
- Comunicação entre agregados
- Desacoplamento de componentes

### Fluxo de Requisição

\`\`\`
Cliente (React)
    ↓
HTTP Request
    ↓
Controller (LeadsController)
    ↓
MediatR
    ↓
Handler (AcceptLeadCommandHandler)
    ↓
Domain Logic (Lead.Accept())
    ↓
Repository (LeadRepository)
    ↓
Entity Framework Core
    ↓
SQL Server
    ↓
Response (JSON)
\`\`\`

### Regras de Negócio Implementadas

1. **Desconto Automático**: Leads com preço > $500 recebem 10% de desconto ao aceitar
2. **Notificação por Email**: Ao aceitar, envia email para vendas@test.com
3. **Status Management**: Leads começam como "Invited", podem ir para "Accepted" ou "Declined"
4. **Imutabilidade de Status**: Uma vez "Accepted" ou "Declined", não pode mudar

### Testing Strategy

- **Unit Tests**: Lógica de domínio (LeadTests)
- **Integration Tests**: Handlers e repositórios
- **Padrão AAA**: Arrange, Act, Assert
- **Mocks**: Isolar dependências externas
*/
