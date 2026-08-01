using KodiakPlugBank.Application.UseCases.Pagador;

namespace KodiakPlugBank.Api.Auth;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AutenticarApikeyFixaUseCase autenticarApikeyFixa)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var apiKey = context.Request.Headers["X-Api-Key"].ToString();
        var resultado = await autenticarApikeyFixa.ExecuteAsync(apiKey, context.RequestAborted);

        if (!resultado.IsSuccess)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = resultado.Error });
            return;
        }

        await _next(context);
    }
}
