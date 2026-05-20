using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace ProcessingSystem.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); 
                if (!context.Response.HasStarted)
                {
                    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                        await WriteCustomJsonErrorAsync(context, HttpStatusCode.Unauthorized, 
                            "No está autorizado. El token JWT es inválido, expiró o no fue enviado.");
                    }
                    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        await WriteCustomJsonErrorAsync(context, HttpStatusCode.Forbidden, 
                            "Acceso denegado. No tienes el rol necesario (Ciudadano) para acceder a este endpoint.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error no controlado en la aplicación: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Ocurrio un error inesperado en el servidor";

            switch (exception)
            {
                case DbUpdateException dbexception:
                    statusCode = HttpStatusCode.BadRequest;
                    message = dbexception.InnerException != null
                        ? $"Error de Base de Datos: {dbexception.InnerException.Message}"
                        : dbexception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                default:                
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var details = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null;

            var response = new ApiErrorResponse(context.Response.StatusCode, message, details);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private async Task WriteCustomJsonErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiErrorResponse((int)statusCode, message, details: null);
            await SendJsonResponseAsync(context, response);
        }

        private async Task SendJsonResponseAsync(HttpContext context, ApiErrorResponse response)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
