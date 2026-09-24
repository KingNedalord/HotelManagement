using HotelManagement.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.WebCore.Infrastructure;

/// <summary>
/// Catches all unhandled exceptions and maps them to RFC-7807 ProblemDetails responses.
/// No internal details are exposed to the caller in production — only safe messages.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            NotFoundException nfe =>
                (StatusCodes.Status404NotFound, "Not Found", nfe.Message),

            UnauthorizedAccessException uae =>
                (StatusCodes.Status401Unauthorized, "Unauthorized", uae.Message),

            ArgumentException ae =>
                (StatusCodes.Status400BadRequest, "Bad Request", ae.Message),

            DbUpdateException =>
                (StatusCodes.Status409Conflict, "Conflict",
                    "A database conflict occurred. The operation could not be completed."),

            _ =>
                (StatusCodes.Status500InternalServerError, "Internal Server Error",
                    "An unexpected error occurred. Please try again later.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
