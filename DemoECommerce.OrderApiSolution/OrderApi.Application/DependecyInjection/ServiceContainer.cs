using eCommerce.SharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependecyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IOrderService, OrderService>(options =>
        {
            options.BaseAddress = new Uri(config.GetValue<string>("ApiGateway:BaseAddress")!);
            options.Timeout = TimeSpan.FromSeconds(1);
        });

        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                string msg = $"OnRetry, Attempt: {args.AttemptNumber}, Outcome: {args.Outcome}";
                LogExceptions.LogToConsole(msg);
                LogExceptions.LogToDebugger(msg);
                return ValueTask.CompletedTask;
            }
        };

        services.AddResiliencePipeline("my-retry-pipeline", builder => { builder.AddRetry(retryStrategy); });

        return services;
    }
}