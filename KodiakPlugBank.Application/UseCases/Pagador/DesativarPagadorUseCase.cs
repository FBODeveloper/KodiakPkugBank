using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class DesativarPagadorUseCase
{
    private readonly IPlugBankApi _plugBankApi;
    private readonly IPagadorRepository _pagadorRepository;

    public DesativarPagadorUseCase(IPlugBankApi plugBankApi, IPagadorRepository pagadorRepository)
    {
        _plugBankApi = plugBankApi;
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<DesativarPayerResponse>> ExecuteAsync(
        string tokenPayer,
        string payerCpfCnpj,
        PlugBankCredentials baseCredentials,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(payerCpfCnpj))
            return Result.Fail<DesativarPayerResponse>("Header payercpfcnpj não informado.", 401);

        var pagador = await _pagadorRepository.GetByCpfCnpjAsync(payerCpfCnpj, cancellationToken);
        if (pagador is null)
            return Result.Fail<DesativarPayerResponse>($"Pagador não encontrado para o payercpfcnpj {payerCpfCnpj}.", 404);

        var credentials = new PlugBankCredentials
        {
            CnpjSh = baseCredentials.CnpjSh,
            TokenSh = baseCredentials.TokenSh,
            PayerCpfCnpj = pagador.CpfCnpj
        };

        DesativarPayerResponse response;
        try
        {
            response = await _plugBankApi.DisablePayerAsync(tokenPayer, credentials, cancellationToken);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<DesativarPayerResponse>(ex.Message, ex.StatusCode);
        }

        pagador.Ativo = false;
        await _pagadorRepository.UpdateAsync(pagador, cancellationToken);

        return Result.Ok(response);
    }
}
