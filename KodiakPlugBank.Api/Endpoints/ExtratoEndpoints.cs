using KodiakPlugBank.Api.Auth;
using KodiakPlugBank.Application.UseCases.Extrato;
using KodiakPlugBank.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace KodiakPlugBank.Api.Endpoints;

public static class ExtratoEndpoints
{
    public static void MapExtratoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/statement/openfinance").WithTags("Statement OpenFinance");

        group.MapPost("/", async (
            CriarExtratoRequest request,
            CriarExtratoUseCase useCase,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var pagador = context.GetPagador();
            var solicitacao = request with { IdPagador = pagador.Id };
            var resultado = await useCase.ExecuteAsync(solicitacao, options.Value.ToCredentials(), ct);
            return resultado.IsSuccess
                ? Results.Accepted($"/api/v1/statement/openfinance/{resultado.Value!.UniqueId}", resultado.Value)
                : ApiResponse.From(resultado);
        })
        .WithName("CreateStatementOpenfinance")
        .WithSummary("Solicita a geração de um extrato bancário via Open Finance.")
        .Produces<CriarExtratoResponse>(StatusCodes.Status202Accepted)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{uniqueId}", async (
            string uniqueId,
            ObterExtratoUseCase useCase,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var pagador = context.GetPagador();
            var resultado = await useCase.ExecuteAsync(uniqueId, pagador.Id, options.Value.ToCredentials(), ct);
            return ApiResponse.From(resultado);
        })
        .WithName("GetStatementOpenfinanceById")
        .WithSummary("Obtém o extrato gerado pelo uniqueId.")
        .Produces<Core.PlugBank.OpenFinance.StatementDocument>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
