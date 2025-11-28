# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ["LeadsManagement.sln", "."]
COPY ["src/LeadsManagement.API/LeadsManagement.API.csproj", "src/LeadsManagement.API/"]
COPY ["src/LeadsManagement.Application/LeadsManagement.Application.csproj", "src/LeadsManagement.Application/"]
COPY ["src/LeadsManagement.Domain/LeadsManagement.Domain.csproj", "src/LeadsManagement.Domain/"]
COPY ["src/LeadsManagement.Infrastructure/LeadsManagement.Infrastructure.csproj", "src/LeadsManagement.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "LeadsManagement.sln"

# Copy source code
COPY . .

# Build
RUN dotnet build "LeadsManagement.sln" -c Release -o /app/build

# Publish
RUN dotnet publish "src/LeadsManagement.API/LeadsManagement.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .

# Generate self-signed certificate
RUN apt-get update && apt-get install -y openssl
RUN openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj "/CN=localhost"
RUN openssl pkcs12 -export -in cert.pem -inkey key.pem -out aspnetapp.pfx -passout pass:password

ENTRYPOINT ["dotnet", "LeadsManagement.API.dll"]
