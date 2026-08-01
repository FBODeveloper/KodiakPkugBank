using System.Net;
using KodiakPlugBank.Api;
using KodiakPlugBank.Api.Auth;
using KodiakPlugBank.Api.Endpoints;
using KodiakPlugBank.Api.Security;
using KodiakPlugBank.Application.UseCases.Conta;
using KodiakPlugBank.Application.UseCases.Extrato;
using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Infrastructure;
using KodiakPlugBank.Infrastructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddPlugBankEnvironmentVariables();

var forwardedSettings = builder.Configuration
    .GetSection(ForwardedHeadersSettings.SectionName)
    .Get<ForwardedHeadersSettings>() ?? new ForwardedHeadersSettings();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = forwardedSettings.ForwardLimit;
    options.KnownProxies.Clear();
    options.KnownIPNetworks.Clear();
    foreach (var proxy in forwardedSettings.KnownProxies)
    {
        options.KnownProxies.Add(IPAddress.Parse(proxy));
    }
    foreach (var network in forwardedSettings.KnownNetworks)
    {
        var parts = network.Split('/');
        var prefix = parts.Length > 1 && int.TryParse(parts[1], out var parsed) ? parsed : 32;
        options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse(parts[0]), prefix));
    }
});

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
builder.Services.AddSecurityRateLimiting(builder.Configuration);

builder.Services.AddScoped<AutenticarPagadorUseCase>();
builder.Services.AddScoped<CriarPagadorUseCase>();
builder.Services.AddScoped<ListarPagadoresUseCase>();
builder.Services.AddScoped<ObterPagadorUseCase>();
builder.Services.AddScoped<CriarContaUseCase>();
builder.Services.AddScoped<ListarContasUseCase>();
builder.Services.AddScoped<CriarExtratoUseCase>();
builder.Services.AddScoped<ObterExtratoUseCase>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kodiak PlugBank API v1");
    });
}

app.UseRateLimiter();

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
