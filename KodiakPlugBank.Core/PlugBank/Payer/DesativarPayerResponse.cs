namespace KodiakPlugBank.Core.PlugBank.Payer;

public class DesativarPayerResponse
{
    public bool? Active { get; set; }
    public string? Message { get; set; }
    public DesativarPayerInfo? Payer { get; set; }
}

public class DesativarPayerInfo
{
    public string? Name { get; set; }
}
