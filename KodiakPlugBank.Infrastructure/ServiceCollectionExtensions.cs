using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Infrastructure.Data;
using KodiakPlugBank.Infrastructure.Data.Repositories;
using KodiakPlugBank.Infrastructure.Options;
using KodiakPlugBank.Infrastructure.PlugBank;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KodiakPlugBank.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<PlugBankOptions>(configuration.GetSection(PlugBankOptions.SectionName));

        services.AddSingleton<DbConnectionFactory>();
        services.AddSingleton<SchemaInitializer>();
        services.AddScoped<IPagadorRepository, PagadorRepository>();
        services.AddScoped<IContaBancariaRepository, ContaBancariaRepository>();

        services.AddHttpClient<IPlugBankApi, PlugBankApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PlugBankOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }
}
