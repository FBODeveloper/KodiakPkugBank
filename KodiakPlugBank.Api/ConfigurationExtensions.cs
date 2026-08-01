namespace KodiakPlugBank.Api;

public static class ConfigurationExtensions
{
    public const string EnvCnpjSh = "KODIAK_PLUGBANK_SH";
    public const string EnvTokenSh = "KODIAK_PLUGBANK";

    public static IConfigurationBuilder AddPlugBankEnvironmentVariables(this IConfigurationBuilder builder)
    {
        var values = new Dictionary<string, string?>();

        var cnpjSh = Environment.GetEnvironmentVariable(EnvCnpjSh);
        if (!string.IsNullOrWhiteSpace(cnpjSh))
            values["PlugBank:CnpjSh"] = cnpjSh;

        var tokenSh = Environment.GetEnvironmentVariable(EnvTokenSh);
        if (!string.IsNullOrWhiteSpace(tokenSh))
            values["PlugBank:TokenSh"] = tokenSh;

        if (values.Count > 0)
            builder.AddInMemoryCollection(values);

        return builder;
    }
}
