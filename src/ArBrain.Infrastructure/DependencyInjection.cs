using ArBrain.Application.Interfaces.Repositories;
using ArBrain.Infrastructure.Data;
using ArBrain.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArBrain.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBeerRepository, BeerRepository>();
        services.AddScoped<ITankRepository, TankRepository>();
        services.AddScoped<IFermentationRecordRepository, FermentationRecordRepository>();

        return services;
    }
}
