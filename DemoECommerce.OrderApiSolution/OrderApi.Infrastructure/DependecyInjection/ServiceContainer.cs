using eCommerce.SharedLibrary.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interface;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Infrastructure.DependecyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config) 
    {
        services.AddSharedServices<OrderDbContext>(config, config["MySerilog:FileName"]!);
        services.AddScoped<IOrder, OrderRepository>();
        return services;
    }
    
    public static IApplicationBuilder UseOrderApiInfrastructure(this IApplicationBuilder app)
    {
        app.UseSharedPolicies();
        return app;
    }
}