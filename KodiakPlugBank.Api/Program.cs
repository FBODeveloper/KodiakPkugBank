using KodiakPlugBank.Api;
using KodiakPlugBank.Api.Auth;
using KodiakPlugBank.Api.Endpoints;
using KodiakPlugBank.Application.UseCases.Conta;
using KodiakPlugBank.Application.UseCases.Extrato;
using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Infrastructure;
using KodiakPlugBank.Infrastructure.Data;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddPlugBankEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kodiak PlugBank API",
        Version = "v1",
        Description = "API de consulta a extratos bancários via Open Finance (integração com a TecnoSped PlugBank)."
    });
});
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AutenticarPagadorUseCase>();
builder.Services.AddScoped<CriarPagadorUseCase>();
builder.Services.AddScoped<ListarPagadoresUseCase>();
builder.Services.AddScoped<ObterPagadorUseCase>();
builder.Services.AddScoped<CriarContaUseCase>();
builder.Services.AddScoped<ListarContasUseCase>();
builder.Services.AddScoped<CriarExtratoUseCase>();
builder.Services.AddScoped<ObterExtratoUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kodiak PlugBank API v1");
    });
}

app.UseMiddleware<ApiKeyMiddleware>();

app.MapPagadorEndpoints();
app.MapContaEndpoints();
app.MapExtratoEndpoints();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var initializer = scope.ServiceProvider.GetRequiredService<SchemaInitializer>();
        await initializer.ApplyAsync();
        app.Logger.LogInformation("Schema do banco de dados verificado/aplicado com sucesso.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao verificar/aplicar o schema do banco de dados.");
    }
}

app.Run();
