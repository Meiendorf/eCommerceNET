using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var message = "Sorry, Internal Server Error occured.";
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var title = "Error";

        try
        {
            await next(context);

            // TODO: this is ugly, but i just follow the course, sorry future me
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too Many Requests";
                statusCode = (int)HttpStatusCode.TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access this resource.";
                statusCode = (int)HttpStatusCode.Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of access";
                message = "You are not allowed to access this resource.";
                statusCode = (int)HttpStatusCode.Forbidden;
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                message = "Request Timed Out";
                title = "Timeout";
                statusCode = StatusCodes.Status408RequestTimeout;
            }
            
            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(
            new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }
        ), CancellationToken.None);
        return;
    }
}