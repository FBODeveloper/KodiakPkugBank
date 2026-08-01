using KodiakPlugBank.Api.Auth;
using KodiakPlugBank.Application.UseCases.Conta;
using KodiakPlugBank.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace KodiakPlugBank.Api.Endpoints;

public static class ContaEndpoints
{
    public static void MapContaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/account").WithTags("Account");

        group.MapPost("/", async (
            CriarContaRequest request,
            CriarContaUseCase useCase,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var pagador = context.GetPagador();
            var criacao = request with { IdPagador = pagador.Id };
            var resultado = await useCase.ExecuteAsync(criacao, options.Value.ToCredentials(), ct);
            return resultado.IsSuccess
                ? Results.Created("/api/v1/account", resultado.Value)
                : ApiResponse.From(resultado);
        })
        .WithName("CreateAccount")
        .WithSummary("Associa contas bancárias ao pagador autenticado na PlugBank e no banco local.")
        .Produces<CriarContaResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", async (
            ListarContasUseCase useCase,
            HttpContext context,
            CancellationToken ct) =>
        {
            var pagador = context.GetPagador();
            var resultado = await useCase.ExecuteAsync(pagador.Id, ct);
            return ApiResponse.From(resultado);
        })
        .WithName("ListAccounts")
        .WithSummary("Lista as contas bancárias do pagador autenticado.")
        .Produces<IEnumerable<ContaResponseItem>>();
    }
}
