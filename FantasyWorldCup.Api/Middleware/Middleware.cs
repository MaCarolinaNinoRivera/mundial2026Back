using System.Net;
using System.Text.Json;

namespace FantasyWorldCup.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleException(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await HandleException(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleException(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            await HandleException(context, HttpStatusCode.ServiceUnavailable, "Un servicio externo no respondió correctamente.");
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenException ex)
        {
            await HandleException(context, HttpStatusCode.Unauthorized, "El token de seguridad es inválido o ha expirado.");
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // Normalmente es un conflicto de datos o error de validación de BD
            await HandleException(context, HttpStatusCode.Conflict, "Error al actualizar la base de datos. Verifique si los datos ya existen.");
        }
        catch (Exception ex)
        {
            await HandleException(context, HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static async Task HandleException(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.StatusCode = (int)status;
        var response = new
        {
            error = message,
            detail = "Consult the logs for more information",
            timestamp = DateTime.UtcNow
        };
        await context.Response.WriteAsJsonAsync(response);
    }
}