using KodiakPlugBank.Api.Auth;
using KodiakPlugBank.Api.Security;
using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Infrastructure.Options;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace KodiakPlugBank.Api.Endpoints;

public static class PagadorEndpoints
{
    public static void MapPagadorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/payer").WithTags("Payer");

        group.MapPost("/", async (
            CriarPagadorRequest request,
            CriarPagadorUseCase useCase,
            IOptions<PlugBankOptions> options,
            CancellationToken ct) =>
        {
            var resultado = await useCase.ExecuteAsync(request, options.Value.ToCredentials(), ct);
            return resultado.IsSuccess
                ? Results.Created($"/api/v1/payer/{resultado.Value!.Id}", resultado.Value)
                : ApiResponse.From(resultado);
        })
        .WithName("CreatePayer")
        .WithSummary("Cadastra um pagador na PlugBank e no banco local.")
        .RequireRateLimiting(PolicyNames.Bootstrap)
        .Produces<PagadorResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status429TooManyRequests)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", async (
            ListarPagadoresUseCase useCase,
            CancellationToken ct) =>
        {
            var resultado = await useCase.ExecuteAsync(ct);
            return ApiResponse.From(resultado);
        })
        .WithName("ListPayers")
        .WithSummary("Lista os pagadores cadastrados.")
        .Produces<IEnumerable<PagadorResponse>>();

        group.MapGet("/{id:int}", async (
            int id,
            ObterPagadorUseCase useCase,
            CancellationToken ct) =>
        {
            var resultado = await useCase.ExecuteAsync(id, ct);
            return ApiResponse.From(resultado);
        })
        .WithName("GetPayerById")
        .WithSummary("Obtém um pagador pelo id.")
        .Produces<PagadorResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
