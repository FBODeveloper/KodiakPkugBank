using KodiakPlugBank.Api.Security;
using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.PlugBank.Payer;
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
            ConsultarPagadorPlugBankUseCase useCase,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var payercpfcnpj = context.Request.Headers["payercpfcnpj"].ToString();
            var resultado = await useCase.ExecuteAsync(payercpfcnpj, options.Value.ToCredentials(), ct);
            return ApiResponse.From(resultado);
        })
        .WithName("GetPayer")
        .WithSummary("Consulta um pagador na PlugBank pelo header payercpfcnpj.")
        .Produces<PayerConsultaResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/list", async (
            ListarPagadoresPlugBankUseCase useCase,
            IOptions<PlugBankOptions> options,
            CancellationToken ct) =>
        {
            var resultado = await useCase.ExecuteAsync(options.Value.ToCredentials(), ct);
            return ApiResponse.From(resultado);
        })
        .WithName("ListPayers")
        .WithSummary("Lista os pagadores na PlugBank.")
        .Produces<PayerListResponse>();

        group.MapPut("/", async (
            CreatePayerRequest request,
            AtualizarPagadorUseCase useCase,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var payercpfcnpj = context.Request.Headers["payercpfcnpj"].ToString();
            var resultado = await useCase.ExecuteAsync(payercpfcnpj, request, options.Value.ToCredentials(), ct);
            return ApiResponse.From(resultado);
        })
        .WithName("UpdatePayer")
        .WithSummary("Atualiza os dados do pagador na PlugBank e no banco local.")
        .Produces<PagadorResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapDelete("/{tokenPayer}", async (
            string tokenPayer,
            DesativarPagadorUseCase useCase,
            IOptions<PlugBankOptions> options,
            HttpContext context,
            CancellationToken ct) =>
        {
            var payercpfcnpj = context.Request.Headers["payercpfcnpj"].ToString();
            var resultado = await useCase.ExecuteAsync(tokenPayer, payercpfcnpj, options.Value.ToCredentials(), ct);
            return ApiResponse.From(resultado);
        })
        .WithName("DisablePayer")
        .WithSummary("Desativa um pagador na PlugBank pelo tokenPayer e marca como inativo no banco local.")
        .Produces<DesativarPayerResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }
}
