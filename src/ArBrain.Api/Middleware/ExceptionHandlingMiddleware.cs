using ArBrain.Application.Exceptions;

namespace ArBrain.Api.Middleware;

/// <summary>
/// Converte exceções de aplicação em respostas HTTP padronizadas.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Erro de validação",
                errors = ex.Errors,
            });
        }
        catch (Exception)
        {
            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Erro interno inesperado.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            title = message,
        });
    }
}
