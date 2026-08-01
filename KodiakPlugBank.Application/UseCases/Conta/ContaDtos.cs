namespace KodiakPlugBank.Application.UseCases.Conta;

public record CriarContaRequestItem(int IdContaBancariaKodiak, Core.PlugBank.Account.CreateAccountItemRequest Conta);

public record CriarContaRequest(int IdPagador, List<CriarContaRequestItem> Contas);

public record CriarContaResponse(int Id, List<ContaResponseItem> Contas);

public record ContaResponseItem(
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
