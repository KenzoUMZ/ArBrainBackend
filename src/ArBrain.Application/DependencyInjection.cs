using ArBrain.Application.Interfaces.Services;
using ArBrain.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArBrain.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBeerService, BeerService>();
        services.AddScoped<ITankService, TankService>();
        services.AddScoped<IFermentationRecordService, FermentationRecordService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
