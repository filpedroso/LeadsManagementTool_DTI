/*
# 📚 GUIA DE INSTALAÇÃO - Leads Management System

## Pré-requisitos

- **.NET 6 SDK** - https://dotnet.microsoft.com/download/dotnet/6.0
- **SQL Server 2022 Express** - https://www.microsoft.com/pt-br/sql-server/sql-server-downloads
- **Node.js 18+** - https://nodejs.org/
- **Git** - https://git-scm.com/
- **Docker** (opcional) - https://www.docker.com/

## Instalação Local (Sem Docker)

### 1. Backend Setup

\`\`\`bash
# Clone o repositório
git clone <seu-repositorio-url>
cd LeadsManagement

# Restaurar dependências
dotnet restore

# Aplicar migrações (cria o banco de dados)
cd src/LeadsManagement.Infrastructure
dotnet ef database update --startup-project ../LeadsManagement.API

# Rodar a API
cd ../LeadsManagement.API
dotnet run
\`\`\`

A API estará disponível em: https://localhost:7000

### 2. Frontend Setup

\`\`\`bash
# Em outra aba do terminal
cd frontend

# Instalar dependências
npm install

# Rodar servidor de desenvolvimento
npm run dev
\`\`\`

O frontend estará disponível em: http://localhost:3000

## Instalação com Docker

\`\`\`bash
# Build e rodar todos os serviços
docker-compose up -d

# Acompanhar logs
docker-compose logs -f

# Parar serviços
docker-compose down
\`\`\`

## Connection String do Banco de Dados

**Desenvolvimento (Local):**
\`\`\`
Server=.;Database=LeadsManagementDb;Trusted_Connection=true;TrustServerCertificate=true;
\`\`\`

**Docker:**
\`\`\`
Server=sqlserver,1433;Database=LeadsManagementDb;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;
\`\`\`

## Verificar Emails Simulados

Os emails simulados são salvos em: `src/LeadsManagement.API/emails/`

## Rodar Testes

\`\`\`bash
# Executar todos os testes
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true
\`\`\`

## Troubleshooting

### Erro de certificado HTTPS
Se receber erro de certificado SSL:

\`\`\`bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
\`\`\`

### Banco de dados não é criado
Verificar se SQL Server está rodando:

\`\`\`bash
# No SSMS, conectar a (.) ou localhost
\`\`\`

### Frontend não conecta na API
Verificar se CORS está habilitado no appsettings.Development.json

### Docker com permissões
Em Linux/Mac, pode ser necessário:

\`\`\`bash
sudo docker-compose up -d
\`\`\`
*/
