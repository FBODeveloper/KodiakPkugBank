namespace KodiakPlugBank.Core.PlugBank.Account;

public class CreateAccountItemRequest
{
    public string BankCode { get; set; } = string.Empty;
    public string Agency { get; set; } = string.Empty;
    public string? AgencyDigit { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? AccountNumberDigit { get; set; }
    public string? AccountDac { get; set; }
    public string? AccountType { get; set; }
    public bool AccountPayment { get; set; }
    public bool GovernmentalResource { get; set; }
    public string? ConvenioAgency { get; set; }
    public string? ConvenioNumber { get; set; }
    public long? RemessaSequential { get; set; }
    public bool Webservice { get; set; }
    public string? CodeContract { get; set; }
    public bool DdaActived { get; set; }
    public string? ClientKey { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientId { get; set; }
    public bool RecipientNotification { get; set; }
    public bool StatementActived { get; set; }
    public bool PagBBEnabled { get; set; }
}
