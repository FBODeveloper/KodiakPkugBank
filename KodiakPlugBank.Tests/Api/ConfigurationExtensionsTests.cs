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
}
