namespace KodiakPlugBank.Core.PlugBank.Payer;

public class AtualizarPayerResponse
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Street { get; set; }
    public string? Neighborhood { get; set; }
    public string? AddressNumber { get; set; }
    public string? AddressComplement { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zipcode { get; set; }
    public bool? StatementActived { get; set; }
}
