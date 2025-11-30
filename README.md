# Leads Management System

A full-stack lead management single-page app

| Tech | Stack |
|------|-------|
| Backend | .NET 6 |
| Frontend | React |
| Database | SQL Server |

## Backend Design Choices

### CQRS with MediatR

Write operations (commands) are separated from read operations (queries). Each handler follows the single responsibility principle.

### Domain Driven Design (DDD)

Applies business-focused architecture with clear separation of concerns across layers:

- **API Layer:** Handles HTTP requests and exposes REST endpoints for CRUD operations.

- **Application Layer:** Implements CQRS operations—Commands (Write) and Queries (Read). Also defines DTOs (Data Transfer Objects) that safely transfer data between layers without exposing internal business logic.

- **Domain Layer:** Contains business logic and rules. Defines the `Lead` entity, its ValueObjects (`Contact`, `Money`), and the `IEmailService` interface for notifications.

- **Infrastructure Layer:** Manages persistence and database state using SQL Server. Uses Entity Framework Core (ORM) to map SQL queries to object-oriented syntax, keeping database concerns isolated from business logic.

---

## Start Guide

Follow these steps to get the project running on a fresh machine.

*(Tested on MacBook Pro mid 2015 - macOS 12.4 Monterey)*

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (v16 or higher)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 1. Clone the Repository

```bash
git clone https://github.com/filpedroso/LeadsManagementTool_DTI.git
cd LeadsManagementTool_DTI/LeadsManagement
```

### 2. Start SQL Server with Docker

Open Docker Desktop, then run:

```bash
# Create and start SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourPassword123!" \
  -p 1433:1433 --name sqlserver -d \
  mcr.microsoft.com/mssql/server:2022-latest

# Verify it's running
docker ps
```

### 3. Backend Setup (.NET)

```bash
# Navigate to the project root
cd LeadsManagement

# Restore .NET dependencies
dotnet restore

# Install Entity Framework tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create and apply database migrations
dotnet ef database update --project src/LeadsManagement.Infrastructure --startup-project src/LeadsManagement.API

# Run backend tests
dotnet test

# Start the API (from project root)
cd src/LeadsManagement.API
dotnet run
```

The API will be available at:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:7000

### 4. Frontend Setup (React)

Open a new terminal:

```bash
# Navigate to frontend directory
cd LeadsManagement/frontend

# Install dependencies
npm install

# Run frontend tests
npm test

# Start the development server
npm run dev
```

The frontend will be available at: **http://localhost:3000**

---

## API Endpoints

Access via Swagger UI at `/swagger`

### Leads

- `GET /api/v1/Leads/status/{status}` - Get leads by status (Invited, Accepted, Declined)
- `GET /api/v1/Leads/{id}` - Get specific lead by its ID
- `POST /api/v1/Leads` - Create a lead (for testing purposes)
- `POST /api/v1/Leads/{id}/accept` - Accept a lead
- `POST /api/v1/Leads/{id}/decline` - Decline a lead

---

## Database Configuration

The connection string is configured in `src/LeadsManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=LeadsManagementDb;User Id=sa;Password=YourPassword123!;Encrypt=False;TrustServerCertificate=True;"
  }
}
```

---

## Project Structure

```
LeadsManagement/
├── src/
│   ├── LeadsManagement.API/              # Web API layer
│   ├── LeadsManagement.Application/      # Application layer (CQRS)
│   ├── LeadsManagement.Domain/           # Domain entities and value objects
│   └── LeadsManagement.Infrastructure/   # Data access and services
├── LeadsManagement.Tests/                # Unit tests
└── frontend/
    ├── src/
    │   ├── api/                          # API client
    │   ├── components/                   # React components
    │   ├── hooks/                        # Custom hooks
    │   └── test/                         # Test utilities
    └── package.json
```

---

## Development Notes

### Frontend Architecture

- **React 18** with functional components and hooks
- **Vite** for fast development and building
- **Axios** for HTTP requests
- **React Toastify** for notifications
- **Vitest + Testing Library** for testing

### Business Rules

- Leads have three statuses: Invited, Accepted, Declined
- Accepted leads receive a 10% discount if price > $500
- Email notifications sent on lead acceptance (fake service for testing)
- Full contact information only visible for Accepted/Declined leads

---

**Fil Pedroso**  
GitHub: [@filpedroso](https://github.com/filpedroso)

This project is part of a technical assessment for DTI Digital.
