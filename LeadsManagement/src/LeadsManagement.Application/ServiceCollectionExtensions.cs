using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LeadsManagement.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceCollectionExtensions));
        return services;
    }
}
