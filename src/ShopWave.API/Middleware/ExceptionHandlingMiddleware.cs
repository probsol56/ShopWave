using System.Text.Json;
using ShopWave.Application.Common.Exceptions;

namespace ShopWave.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errors) = exception switch
        {
            ValidationException ve => (StatusCodes.Status422UnprocessableEntity, ve.Errors),
            NotFoundException => (StatusCodes.Status404NotFound, (IDictionary<string, string[]>?)null),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, null),
            ForbiddenException => (StatusCodes.Status403Forbidden, null),
            InvalidOperationException => (StatusCodes.Status400BadRequest, null),
            _ => (StatusCodes.Status500InternalServerError, null)
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            status = statusCode,
            message = exception.Message,
            errors
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
