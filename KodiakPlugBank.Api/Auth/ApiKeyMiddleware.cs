using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.Entities;

namespace KodiakPlugBank.Api.Auth;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _masterApiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _masterApiKey = configuration["Security:MasterApiKey"];
    }

    public async Task InvokeAsync(HttpContext context, AutenticarPagadorUseCase autenticarPagador)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var method = context.Request.Method;

        if (path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var apiKey = context.Request.Headers["X-Api-Key"].ToString();

        if (IsBootstrapEndpoint(path, method))
        {
            if (string.IsNullOrEmpty(_masterApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { message = "Chave mestre não configurada no servidor." });
                return;
            }

            if (apiKey != _masterApiKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Chave de acesso inválida." });
                return;
            }

            await _next(context);
            return;
        }

        var resultado = await autenticarPagador.ExecuteAsync(apiKey, context.RequestAborted);
        if (!resultado.IsSuccess || resultado.Value is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = resultado.Error });
            return;
        }

        context.Items[HttpContextItemKeys.Pagador] = resultado.Value;
        await _next(context);
    }

    private static bool IsBootstrapEndpoint(string path, string method) =>
        method == HttpMethods.Post && path.Equals("/api/v1/payer", StringComparison.OrdinalIgnoreCase);
}

public static class HttpContextItemKeys
{
    public const string Pagador = "Pagador";
}

public static class HttpContextExtensions
{
    public static Pagador GetPagador(this HttpContext context) =>
        context.Items[HttpContextItemKeys.Pagador] as Pagador
        ?? throw new InvalidOperationException("Pagador autenticado não encontrado no contexto.");
}
