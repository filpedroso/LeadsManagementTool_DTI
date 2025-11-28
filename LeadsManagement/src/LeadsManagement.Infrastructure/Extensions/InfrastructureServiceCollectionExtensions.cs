using LeadsManagement.Infrastructure.Data.Contexts;
using LeadsManagement.Infrastructure.Data.Repositories;
using LeadsManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeadsManagement.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<LeadRepository>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
