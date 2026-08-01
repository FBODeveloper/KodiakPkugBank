using KodiakPlugBank.Core.PlugBank.Common;

namespace KodiakPlugBank.Infrastructure.Options;

public class PlugBankOptions
{
    public const string SectionName = "PlugBank";
    public string BaseUrl { get; set; } = "https://api.pagamentobancario.com.br";
    public string CnpjSh { get; set; } = string.Empty;
    public string TokenSh { get; set; } = string.Empty;

    public PlugBankCredentials ToCredentials(string? payerCpfCnpj = null) => new()
    {
        CnpjSh = CnpjSh,
        TokenSh = TokenSh,
        PayerCpfCnpj = payerCpfCnpj
    };
}
