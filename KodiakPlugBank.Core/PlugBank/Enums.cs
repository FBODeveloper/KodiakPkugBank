using System.Text.Json.Serialization;

namespace KodiakPlugBank.Core.PlugBank;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatementType
{
    BANK,
    CREDIT_CARD
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType
{
    credit,
    debit
}
