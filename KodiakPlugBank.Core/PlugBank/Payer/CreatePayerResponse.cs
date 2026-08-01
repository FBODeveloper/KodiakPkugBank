namespace KodiakPlugBank.Core.PlugBank.Payer;

public class CreatePayerResponse
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? CpfCnpj { get; set; }
    public List<PayerResponseAccount>? Accounts { get; set; }
    public string? Street { get; set; }
    public string? Neighborhood { get; set; }
    public string? AddressNumber { get; set; }
    public string? AddressComplement { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zipcode { get; set; }
    public string? Token { get; set; }
    public bool? StatementActived { get; set; }
}

public class PayerResponseAccount
{
    public string? BankCode { get; set; }
    public string? AccountHash { get; set; }
    public string? Agency { get; set; }
    public string? AgencyDigit { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountNumberDigit { get; set; }
    public string? AccountDac { get; set; }
    public string? ConvenioAgency { get; set; }
    public string? ConvenioNumber { get; set; }
    public long? RemessaSequential { get; set; }
    public long? AccountType { get; set; }
}
