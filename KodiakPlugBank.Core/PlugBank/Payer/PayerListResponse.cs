using System.Text.Json.Serialization;

namespace KodiakPlugBank.Core.PlugBank.Payer;

public class PayerListResponse
{
    public List<PayerListItem>? Payers { get; set; }
}

public class PayerListItem
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Token { get; set; }
    public string? Street { get; set; }
    public string? Neighborhood { get; set; }
    public string? AddressNumber { get; set; }
    public string? AddressComplement { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zipcode { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("stamentActived")]
    public bool? StatementActived { get; set; }
}
