using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware;

public class ListenOnlyToApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var signedHeader = context.Request.Headers["X-API-GATEWAY"];
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Sorry, service in unavailable.");
            return;
        }

        await next(context);
    }
}