namespace KodiakPlugBank.Application.UseCases.Pagador;

public record CriarPagadorRequest(
    string Name,
    string? Email,
    string CpfCnpj,
    bool? DdaActived,
    bool? StatementActived,
    string Street,
    string Neighborhood,
    string? AddressNumber,
    string? AddressComplement,
    string City,
    string State,
    string Zipcode,
    List<Core.PlugBank.Payer.PayerAccount>? Accounts,
    string ChaveKodiakExtrato);

public record PagadorResponse(
    int Id,
    string Nome,
    string? Email,
    string CpfCnpj,
    string ChaveKodiakExtrato,
    string? Token,
    bool StatementAtivado);

public record ContaResponse(
    int Id,
    int IdContaBancariaKodiak,
    string? AccountHash,
    string BankCode,
    string Agency,
    string? AgencyDigit,
    string AccountNumber,
    string? AccountNumberDigit,
    bool StatementAtivado,
    string? OpenFinanceLink);
