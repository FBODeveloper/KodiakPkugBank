using System.Text.Json.Serialization;

namespace KodiakPlugBank.Core.PlugBank.OpenFinance;

public class StatementDocument
{
    public StatementInfo? Statement { get; set; }
    public TransactionGroup? Transaction { get; set; }
    public TransactionGroup? TransactionDuplicated { get; set; }
    public StatementBalance? Balance { get; set; }
}

public class StatementInfo
{
    public string? UniqueId { get; set; }
    public string? DateStart { get; set; }
    public string? DateEnd { get; set; }
    public string? BankCode { get; set; }
    public string? TotalTransactions { get; set; }
    public string? Origin { get; set; }
    public string? AccountHash { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
    public string? Type { get; set; }
    public string? CreditCardCurrentAvaliableCreditLimit { get; set; }
    public string? CreditCardCurrentCreditLimit { get; set; }
}

public class TransactionGroup
{
    public List<StatementTransaction>? Credit { get; set; }
    public List<StatementTransaction>? Debit { get; set; }
}

public class StatementTransaction
{
    public string? TransactionId { get; set; }
    public string? TransactionType { get; set; }
    public string? Code { get; set; }
    public string? Amount { get; set; }
    public string? Date { get; set; }
    public long? Sequence { get; set; }
    public string? Description { get; set; }
    public string? Fitid { get; set; }
    public decimal? CreditCardAmountInAccountCurrency { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? PaymentName { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentBarcode { get; set; }
    public string? PaymentBaseAmount { get; set; }
    public string? PaymentDigitableLine { get; set; }
    public string? PaymentDiscountAmount { get; set; }
    public string? PaymentInterestAmount { get; set; }
    public string? PaymentPenaltyAmount { get; set; }
    public TransactionParticipant? ParticipantPayer { get; set; }
    public TransactionParticipant? ParticipantReceiver { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CreditCardNumber { get; set; }
    public CreditCardBill? CreditCardBill { get; set; }
    public CreditCardMerchant? CreditCardMerchant { get; set; }
    public int? CreditCardInstallmentNumber { get; set; }
    public int? CreditCardTotalInstallments { get; set; }
    public string? CreditCardTotalAmount { get; set; }
}

public class TransactionParticipant
{
    public string? Name { get; set; }
    public string? BranchNumer { get; set; }
    public string? AccountNumber { get; set; }
    public string? RoutingNumber { get; set; }
    public DocumentNumber? DocumentNumber { get; set; }
}

public class DocumentNumber
{
    public string? Type { get; set; }
    public string? Value { get; set; }
}

public class CreditCardBill
{
    public string? Name { get; set; }
    public string? BranchNumer { get; set; }
    public string? AccountNumber { get; set; }
    public string? RoutingNumber { get; set; }
    public DocumentNumber? DocumentNumber { get; set; }
    public string? ReferenceNumber { get; set; }
}

public class CreditCardMerchant
{
    public string? Cnae { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Category { get; set; }
    public string? Name { get; set; }
}

public class StatementBalance
{
    public BalancePoint? Inicial { get; set; }
    public BalancePoint? Final { get; set; }
}

public class BalancePoint
{
    public string? Date { get; set; }
    public string? Balance { get; set; }
}
