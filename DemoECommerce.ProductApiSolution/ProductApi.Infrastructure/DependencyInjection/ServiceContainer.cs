using eCommerce.SharedLibrary.DI;
using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddProductApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedServices<ProductDbContext>(configuration, configuration["MySerilog:FileName"]!);
        services.AddScoped<IProduct, ProductRepository>();
        return services;
    }

    public static IApplicationBuilder UseProductApiInfrastructure(this IApplicationBuilder app)
    {
        app.UseSharedPolicies();
        return app;
    }
}