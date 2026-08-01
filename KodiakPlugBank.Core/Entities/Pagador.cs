namespace KodiakPlugBank.Core.Entities;

public class Pagador
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string CpfCnpj { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string? NumeroEndereco { get; set; }
    public string? ComplementoEndereco { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string? Token { get; set; }
    public bool StatementAtivado { get; set; }
    public string ChaveKodiakExtrato { get; set; } = string.Empty;
}
