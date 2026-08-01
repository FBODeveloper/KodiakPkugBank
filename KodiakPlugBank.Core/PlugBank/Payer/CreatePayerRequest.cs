namespace KodiakPlugBank.Core.PlugBank.Payer;

public class CreatePayerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string CpfCnpj { get; set; } = string.Empty;
    public bool? DdaActived { get; set; }
    public bool? StatementActived { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string? AddressNumber { get; set; }
    public string? AddressComplement { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zipcode { get; set; } = string.Empty;
    public List<PayerAccount>? Accounts { get; set; }
}

public class PayerAccount
{
    public string BankCode { get; set; } = string.Empty;
    public string Agency { get; set; } = string.Empty;
    public string? AgencyDigit { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountNumberDigit { get; set; }
    public string? AccountDac { get; set; }
    public string? ConvenioAgency { get; set; }
    public string? ConvenioNumber { get; set; }
    public long? RemessaSequential { get; set; }
}
