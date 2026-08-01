using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;

namespace KodiakPlugBank.Application.UseCases.Extrato;

public class CriarExtratoUseCase
{
    private readonly IPlugBankApi _plugBankApi;
    private readonly IPagadorRepository _pagadorRepository;
    private readonly IContaBancariaRepository _contaBancariaRepository;

    public CriarExtratoUseCase(
        IPlugBankApi plugBankApi,
        IPagadorRepository pagadorRepository,
        IContaBancariaRepository contaBancariaRepository)
    {
        _plugBankApi = plugBankApi;
        _pagadorRepository = pagadorRepository;
        _contaBancariaRepository = contaBancariaRepository;
    }

    public async Task<Result<CriarExtratoResponse>> ExecuteAsync(
        CriarExtratoRequest request,
        PlugBankCredentials baseCredentials,
        CancellationToken cancellationToken = default)
    {
        var pagador = await _pagadorRepository.GetByIdAsync(request.IdPagador, cancellationToken);
        if (pagador is null)
            return Result.Fail<CriarExtratoResponse>("Pagador não encontrado.", 404);

        if (!string.IsNullOrWhiteSpace(request.AccountHash))
        {
            var conta = await _contaBancariaRepository.GetByAccountHashAsync(request.AccountHash, cancellationToken);
            if (conta is null)
                return Result.Fail<CriarExtratoResponse>("Conta não encontrada.", 404);
            if (conta.IdPagador != pagador.Id)
                return Result.Fail<CriarExtratoResponse>("A conta informada não pertence ao pagador.", 403);
        }

        var credentials = new PlugBankCredentials
        {
            CnpjSh = baseCredentials.CnpjSh,
            TokenSh = baseCredentials.TokenSh,
            PayerCpfCnpj = pagador.CpfCnpj
        };

        CreateStatementResponse response;
        try
        {
            response = await _plugBankApi.CreateStatementAsync(new CreateStatementRequest
            {
                AccountHash = request.AccountHash,
                Today = request.Today,
                DateStart = request.DateStart,
                DateEnd = request.DateEnd,
                StatementType = request.StatementType,
                CardNumber = request.CardNumber
            }, credentials, cancellationToken);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<CriarExtratoResponse>(ex.Message, ex.StatusCode);
        }

        return Result.Ok(new CriarExtratoResponse(response.UniqueId, response.Type));
    }
}
