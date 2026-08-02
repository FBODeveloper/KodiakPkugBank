using KodiakPlugBank.Api;
using PlugBankConfig = KodiakPlugBank.Api.ConfigurationExtensions;
using Microsoft.Extensions.Configuration;

namespace KodiakPlugBank.Tests.Api;

public class ConfigurationExtensionsTests
{
    [Fact]
    public void DeveMapearVariaveisDeAmbienteParaConfiguracao()
    {
        Environment.SetEnvironmentVariable(PlugBankConfig.EnvCnpjSh, "cnpjsh-env");
        Environment.SetEnvironmentVariable(PlugBankConfig.EnvTokenSh, "tokensh-env");

        try
        {
            var config = new ConfigurationBuilder()
                .AddPlugBankEnvironmentVariables()
                .Build();

            Assert.Equal("cnpjsh-env", config["PlugBank:CnpjSh"]);
            Assert.Equal("tokensh-env", config["PlugBank:TokenSh"]);
        }
        finally
        {
            Environment.SetEnvironmentVariable(PlugBankConfig.EnvCnpjSh, null);
            Environment.SetEnvironmentVariable(PlugBankConfig.EnvTokenSh, null);
        }
    }

    [Fact]
    public void DeveIgnorarQuandoVariaveisNaoDefinidas()
    {
        Environment.SetEnvironmentVariable(PlugBankConfig.EnvCnpjSh, null);
        Environment.SetEnvironmentVariable(PlugBankConfig.EnvTokenSh, null);

        var config = new ConfigurationBuilder()
            .AddPlugBankEnvironmentVariables()
            .Build();

        Assert.Null(config["PlugBank:CnpjSh"]);
        Assert.Null(config["PlugBank:TokenSh"]);
    }

    [Fact]
    public void DeveRespeitarPrecedenciaDaVariavelDeAmbienteSobreAppSettings()
    {
        Environment.SetEnvironmentVariable(PlugBankConfig.EnvCnpjSh, "cnpjsh-env");

        try
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["PlugBank:CnpjSh"] = "cnpjsh-appsettings" })
                .AddPlugBankEnvironmentVariables()
                .Build();

            Assert.Equal("cnpjsh-env", config["PlugBank:CnpjSh"]);
        }
        finally
        {
            Environment.SetEnvironmentVariable(PlugBankConfig.EnvCnpjSh, null);
        }
    }

    [Fact]
    public void DeveUsarBaseUrlDeStagingQuandoAmbienteNaoDefinido()
    {
        var config = CriarConfiguracao([]);

        Assert.Equal("https://staging.pagamentobancario.com.br", config["PlugBank:BaseUrl"]);
    }

    [Fact]
    public void DeveUsarBaseUrlDeProducaoQuandoAmbienteProduction()
    {
        var config = CriarConfiguracao(["Production"]);

        Assert.Equal("https://api.pagamentobancario.com.br", config["PlugBank:BaseUrl"]);
    }

    [Fact]
    public void DeveRespeitarPrecedenciaDaVariavelDeAmbienteSobreBaseUrlDeProducao()
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(CriarConfiguracao(["Production"]))
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PlugBank:BaseUrl"] = "https://outro.host" })
            .Build();

        Assert.Equal("https://outro.host", config["PlugBank:BaseUrl"]);
    }

    private static IConfiguration CriarConfiguracao(string[] ambientes)
    {
        var raizProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(raizProjeto, "KodiakPlugBank.Api"))
            .AddJsonFile("appsettings.json", optional: false);

        foreach (var ambiente in ambientes)
        {
            builder.AddJsonFile($"appsettings.{ambiente}.json", optional: false);
        }

        return builder.Build();
    }
}
