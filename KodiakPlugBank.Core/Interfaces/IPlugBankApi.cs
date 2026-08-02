using KodiakPlugBank.Core.PlugBank.Account;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Core.Interfaces;

public interface IPlugBankApi
{
    Task<CreatePayerResponse> CreatePayerAsync(CreatePayerRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<PayerConsultaResponse> GetPayerAsync(string payerCpfCnpj, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<PayerListResponse> ListPayersAsync(PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<AtualizarPayerResponse> UpdatePayerAsync(CreatePayerRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<DesativarPayerResponse> DisablePayerAsync(string tokenPayer, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<CreateAccountResponse> CreateAccountAsync(IReadOnlyList<CreateAccountItemRequest> request, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<CreateStatementResponse> CreateStatementAsync(CreateStatementRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
    Task<StatementDocument> GetStatementAsync(string uniqueId, PlugBankCredentials credentials, CancellationToken cancellationToken = default);
}
