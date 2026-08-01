namespace KodiakPlugBank.Core.PlugBank.Account;

public class CreateAccountResponse
{
    public List<CreateAccountResponseItem>? Accounts { get; set; }
}

public class CreateAccountResponseItem
{
    public string? BankCode { get; set; }
    public string? AccountHash { get; set; }
    public string? Agency { get; set; }
    public string? AgencyDigit { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountNumberDigit { get; set; }
    public string? ConvenioAgency { get; set; }
    public string? ConvenioNumber { get; set; }
    public long? RemessaSequential { get; set; }
    public bool? AccountPayment { get; set; }
    public bool? GovernmentalResource { get; set; }
    public bool? StatementActived { get; set; }
    public string? OpenfinanceLink { get; set; }
}
