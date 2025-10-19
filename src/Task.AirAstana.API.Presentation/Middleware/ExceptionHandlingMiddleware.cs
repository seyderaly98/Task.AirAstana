using System;
using System.Net;
using System.Text.Json;
using Task.AirAstana.API.Presentation.Models;
using Task.AirAstana.Domain.Exceptions;

namespace Task.AirAstana.API.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async System.Threading.Tasks.Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            ValidationException validationEx => new ErrorResponse
            {
                Success = false,
                Message = string.Join("; ",validationEx.Errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")),
                StatusCode = (int)HttpStatusCode.BadRequest,
                Timestamp = DateTimeOffset.UtcNow
            },
            NotFoundException notFoundEx => new ErrorResponse
            {
                Success = false,
                Message = notFoundEx.Message,
                StatusCode = (int)HttpStatusCode.NotFound,
                Timestamp = DateTimeOffset.UtcNow
            },
            DomainException domainEx => new ErrorResponse
            {
                Success = false,
                Message = domainEx.Message,
                StatusCode = (int)HttpStatusCode.BadRequest,
                Timestamp = DateTimeOffset.UtcNow
            },
            UnauthorizedAccessException => new ErrorResponse
            {
                Success = false,
                Message = "Нет доступа к данному ресурсу",
                StatusCode = (int)HttpStatusCode.Forbidden,
                Timestamp = DateTimeOffset.UtcNow
            },
            _ => new ErrorResponse
            {
                Success = false,
                Message = "Внутренняя ошибка сервера",
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Timestamp = DateTimeOffset.UtcNow
            }
        };

        response.StatusCode = errorResponse.StatusCode;

        _logger.LogError(exception,
            "Ошибка обработки запроса. Путь: {Path}, Метод: {Method}, StatusCode: {StatusCode}, Сообщение: {Message}",
            context.Request.Path,
            context.Request.Method,
            response.StatusCode,
            exception.Message);

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}