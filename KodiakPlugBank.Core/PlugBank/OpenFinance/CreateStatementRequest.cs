using System.Text.Json.Serialization;

namespace KodiakPlugBank.Core.PlugBank.OpenFinance;

public class CreateStatementRequest
{
    public string? AccountHash { get; set; }
    public bool? Today { get; set; }
    public string? DateStart { get; set; }
    public string? DateEnd { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatementType? StatementType { get; set; }
    public string? CardNumber { get; set; }
}
