using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;

namespace KodiakPlugBank.Application.UseCases.Extrato;

public class ObterExtratoUseCase
{
    private readonly IPlugBankApi _plugBankApi;
    private readonly IPagadorRepository _pagadorRepository;

    public ObterExtratoUseCase(IPlugBankApi plugBankApi, IPagadorRepository pagadorRepository)
    {
        _plugBankApi = plugBankApi;
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<StatementDocument>> ExecuteAsync(
        string uniqueId,
        int idPagador,
        PlugBankCredentials baseCredentials,
        CancellationToken cancellationToken = default)
    {
        var pagador = await _pagadorRepository.GetByIdAsync(idPagador, cancellationToken);
        if (pagador is null)
            return Result.Fail<StatementDocument>("Pagador não encontrado.", 404);

        var credentials = new PlugBankCredentials
        {
            CnpjSh = baseCredentials.CnpjSh,
            TokenSh = baseCredentials.TokenSh,
            PayerCpfCnpj = pagador.CpfCnpj
        };

        StatementDocument response;
        try
        {
            response = await _plugBankApi.GetStatementAsync(uniqueId, credentials, cancellationToken);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<StatementDocument>(ex.Message, ex.StatusCode);
        }

        return Result.Ok(response);
    }
}
