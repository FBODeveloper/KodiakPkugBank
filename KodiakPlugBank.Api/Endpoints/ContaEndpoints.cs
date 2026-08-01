using KodiakPlugBank.Application.UseCases.Conta;
using KodiakPlugBank.Application.UseCases.Pagador;
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
            ObterPagadorPorCpfCnpjUseCase obterPagador,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var pagador = await ObterPagadorDoContexto(context, obterPagador, ct);
            if (pagador is null)
                return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: "payercpfcnpj não informado.");

            var criacao = request with { IdPagador = pagador.Id };
            var resultado = await useCase.ExecuteAsync(criacao, options.Value.ToCredentials(), ct);
            return resultado.IsSuccess
                ? Results.Created("/api/v1/account", resultado.Value)
                : ApiResponse.From(resultado);
        })
        .WithName("CreateAccount")
        .WithSummary("Associa contas bancárias ao pagador identificado pelo header payercpfcnpj.")
        .Produces<CriarContaResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", async (
            ListarContasUseCase useCase,
            ObterPagadorPorCpfCnpjUseCase obterPagador,
            HttpContext context,
            CancellationToken ct) =>
        {
            var pagador = await ObterPagadorDoContexto(context, obterPagador, ct);
            if (pagador is null)
                return Results.Problem(statusCode: StatusCodes.Status401Unauthorized, title: "payercpfcnpj não informado.");

            var resultado = await useCase.ExecuteAsync(pagador.Id, ct);
            return ApiResponse.From(resultado);
        })
        .WithName("ListAccounts")
        .WithSummary("Lista as contas bancárias do pagador identificado pelo header payercpfcnpj.")
        .Produces<IEnumerable<ContaResponseItem>>();
    }

    private static async Task<Core.Entities.Pagador?> ObterPagadorDoContexto(
        HttpContext context,
        ObterPagadorPorCpfCnpjUseCase obterPagador,
        CancellationToken ct)
    {
        var payercpfcnpj = context.Request.Headers["payercpfcnpj"].ToString();
        var resultado = await obterPagador.ExecuteAsync(payercpfcnpj, ct);
        return resultado.IsSuccess ? resultado.Value : null;
    }
}
