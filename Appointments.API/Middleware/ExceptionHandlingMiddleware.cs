using Appointments.Application.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Appointments.API.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var statusCode = HttpStatusCode.InternalServerError;
        var title = "Internal Server Error";
        object details = new { Detail = "An unexpected error occurred. Please try again later." };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                title = "Not Found";
                details = new { Detail = notFoundEx.Message };
                break;

            case BadRequestException badRequestEx:
                statusCode = HttpStatusCode.BadRequest;
                title = "Bad Request";
                details = new { Detail = badRequestEx.Message };
                break;

            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                title = "One or more validation errors occurred.";
                var validationErrors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                details = new { Errors = validationErrors };
                break;
        }

        response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new
        {
            Title = title,
            Status = response.StatusCode,
            Details = details
        });

        await response.WriteAsync(result);
    }
}
