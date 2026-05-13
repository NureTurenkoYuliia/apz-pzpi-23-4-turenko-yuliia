using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CleanArium.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.InnerException?.Message ?? ex.Message
            });
        }
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Unhandled exception");

        //    context.Response.ContentType = "application/json";

        //    int statusCode;
        //    string message;

        //    switch (ex)
        //    {
        //        case DbUpdateException dbEx:
        //            statusCode = StatusCodes.Status400BadRequest;
        //            message = dbEx.InnerException?.Message ?? dbEx.Message;
        //            break;

        //        case KeyNotFoundException:
        //            statusCode = StatusCodes.Status404NotFound;
        //            message = ex.Message;
        //            break;

        //        case UnauthorizedAccessException:
        //            statusCode = StatusCodes.Status401Unauthorized;
        //            message = ex.Message;
        //            break;

        //        case ArgumentException:
        //            statusCode = StatusCodes.Status400BadRequest;
        //            message = ex.Message;
        //            break;

        //        default:
        //            statusCode = StatusCodes.Status500InternalServerError;
        //            message = "Internal server error";
        //            break;
        //    }

        //    context.Response.StatusCode = statusCode;

        //    await context.Response.WriteAsJsonAsync(new
        //    {
        //        error = message
        //    });
        //}
    }
}
