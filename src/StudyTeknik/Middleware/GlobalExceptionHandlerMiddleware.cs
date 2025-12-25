using System.Text.Json;
using FluentValidation;
using Application.Common.Results;

namespace StudyTeknik.Middleware
{
    /// <summary>
    /// Global Exception Handler Middleware
    /// Catches all exceptions and returns appropriate HTTP responses
    /// Specifically handles FluentValidation.ValidationException
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new object();

            switch (exception)
            {
                // 🔴 FluentValidation - returnera 400 Bad Request
                case ValidationException validationEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var errors = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    
                    response = new
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors = errors,
                        timestamp = DateTime.UtcNow
                    };
                    break;

                // 🟡 Argument Exceptions - returnera 400 Bad Request
                case ArgumentException argEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = argEx.Message,
                        timestamp = DateTime.UtcNow
                    };
                    break;

                // 🔵 Key Not Found - returnera 404 Not Found
                case KeyNotFoundException notFoundEx:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = notFoundEx.Message,
                        timestamp = DateTime.UtcNow
                    };
                    break;

                // ⚫ Övriga exceptions - returnera 500 Internal Server Error
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new
                    {
                        statusCode = StatusCodes.Status500InternalServerError,
                        message = "An internal server error occurred",
                        detail = exception.Message,
                        timestamp = DateTime.UtcNow
                    };
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}

