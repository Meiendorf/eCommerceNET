using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace eCommerce.SharedLibrary.DI;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
        IConfiguration config, string fileName) where TContext : DbContext
    {
        services.AddDbContext<TContext>(
            option => option.UseSqlServer(
                config.GetConnectionString("eCommerceConnection"),
                sqlOptions => { sqlOptions.EnableRetryOnFailure(); }));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.File(path: $"{fileName}-.log",
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        services.AddJWTAuthenticationScheme(config);

        return services;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalException>();
        // app.UseMiddleware<ListenOnlyToApiGateway>();
        return app;
    }
}