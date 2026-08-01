namespace KodiakPlugBank.Core.PlugBank.Common;

public class PlugBankCredentials
{
    public string CnpjSh { get; set; } = string.Empty;
    public string TokenSh { get; set; } = string.Empty;
    public string? PayerCpfCnpj { get; set; }
}

public class PlugBankError
{
    public int? Code { get; set; }
    public string? Message { get; set; }
    public List<PlugBankErrorDetail>? Errors { get; set; }
}

public class PlugBankErrorDetail
{
    public string? Message { get; set; }
    public int? InternalCode { get; set; }
}
