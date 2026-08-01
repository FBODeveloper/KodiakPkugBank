namespace KodiakPlugBank.Core.Entities;

public class ContaBancaria
{
    public int Id { get; set; }
    public int IdPagador { get; set; }
    public int IdContaBancariaKodiak { get; set; }
    public string? AccountHash { get; set; }
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
    public bool DdaAtivado { get; set; }
    public string? ClientKey { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientId { get; set; }
    public bool RecipientNotification { get; set; }
    public bool StatementAtivado { get; set; }
    public bool PagBBAtivado { get; set; }
    public string? OpenFinanceLink { get; set; }
}
