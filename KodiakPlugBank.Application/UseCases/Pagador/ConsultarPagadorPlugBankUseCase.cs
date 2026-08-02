using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class ConsultarPagadorPlugBankUseCase
{
    private readonly IPlugBankApi _plugBankApi;

    public ConsultarPagadorPlugBankUseCase(IPlugBankApi plugBankApi)
    {
        _plugBankApi = plugBankApi;
    }

    public async Task<Result<PayerConsultaResponse>> ExecuteAsync(
        string payerCpfCnpj,
        PlugBankCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(payerCpfCnpj))
            return Result.Fail<PayerConsultaResponse>("Header payercpfcnpj não informado.", 401);

        try
        {
            var response = await _plugBankApi.GetPayerAsync(payerCpfCnpj, credentials, cancellationToken);
            return Result.Ok(response);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<PayerConsultaResponse>(ex.Message, ex.StatusCode);
        }
    }
}
