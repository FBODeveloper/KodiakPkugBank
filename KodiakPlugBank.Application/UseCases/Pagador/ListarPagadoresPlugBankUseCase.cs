using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class ListarPagadoresPlugBankUseCase
{
    private readonly IPlugBankApi _plugBankApi;

    public ListarPagadoresPlugBankUseCase(IPlugBankApi plugBankApi)
    {
        _plugBankApi = plugBankApi;
    }

    public async Task<Result<PayerListResponse>> ExecuteAsync(
        PlugBankCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _plugBankApi.ListPayersAsync(credentials, cancellationToken);
            return Result.Ok(response);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<PayerListResponse>(ex.Message, ex.StatusCode);
        }
    }
}
