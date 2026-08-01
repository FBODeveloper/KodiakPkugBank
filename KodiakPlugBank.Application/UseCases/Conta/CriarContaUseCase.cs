using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Account;
using KodiakPlugBank.Core.PlugBank.Common;

namespace KodiakPlugBank.Application.UseCases.Conta;

public class CriarContaUseCase
{
    private readonly IPlugBankApi _plugBankApi;
    private readonly IPagadorRepository _pagadorRepository;
    private readonly IContaBancariaRepository _contaBancariaRepository;

    public CriarContaUseCase(
        IPlugBankApi plugBankApi,
        IPagadorRepository pagadorRepository,
        IContaBancariaRepository contaBancariaRepository)
    {
        _plugBankApi = plugBankApi;
        _pagadorRepository = pagadorRepository;
        _contaBancariaRepository = contaBancariaRepository;
    }

    public async Task<Result<CriarContaResponse>> ExecuteAsync(
        CriarContaRequest request,
        PlugBankCredentials baseCredentials,
        CancellationToken cancellationToken = default)
    {
        var pagador = await _pagadorRepository.GetByIdAsync(request.IdPagador, cancellationToken);
        if (pagador is null)
            return Result.Fail<CriarContaResponse>("Pagador não encontrado.", 404);

        if (request.Contas is null || request.Contas.Count == 0)
            return Result.Fail<CriarContaResponse>("Informe ao menos uma conta.");

        var credentials = new PlugBankCredentials
        {
            CnpjSh = baseCredentials.CnpjSh,
            TokenSh = baseCredentials.TokenSh,
            PayerCpfCnpj = pagador.CpfCnpj
        };

        CreateAccountResponse response;
        try
        {
            response = await _plugBankApi.CreateAccountAsync(
                request.Contas.Select(c => c.Conta).ToList(),
                credentials,
                cancellationToken);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<CriarContaResponse>(ex.Message, ex.StatusCode);
        }

        var resultado = new List<ContaResponseItem>();
        var retorno = response.Accounts ?? new List<CreateAccountResponseItem>();

        for (var i = 0; i < retorno.Count; i++)
        {
            var item = retorno[i];
            var solicitada = request.Contas[i];

            var conta = new Core.Entities.ContaBancaria
            {
                IdPagador = pagador.Id,
                IdContaBancariaKodiak = solicitada.IdContaBancariaKodiak,
                AccountHash = item.AccountHash,
                BankCode = item.BankCode ?? solicitada.Conta.BankCode,
                Agency = item.Agency ?? solicitada.Conta.Agency,
                AgencyDigit = item.AgencyDigit ?? solicitada.Conta.AgencyDigit,
                AccountNumber = item.AccountNumber ?? solicitada.Conta.AccountNumber,
                AccountNumberDigit = item.AccountNumberDigit ?? solicitada.Conta.AccountNumberDigit,
                AccountDac = solicitada.Conta.AccountDac,
                ConvenioAgency = item.ConvenioAgency ?? solicitada.Conta.ConvenioAgency,
                ConvenioNumber = item.ConvenioNumber ?? solicitada.Conta.ConvenioNumber,
                RemessaSequential = item.RemessaSequential ?? solicitada.Conta.RemessaSequential,
                AccountPayment = item.AccountPayment ?? solicitada.Conta.AccountPayment,
                GovernmentalResource = item.GovernmentalResource ?? solicitada.Conta.GovernmentalResource,
                StatementAtivado = item.StatementActived ?? solicitada.Conta.StatementActived,
                OpenFinanceLink = item.OpenfinanceLink,
                AccountType = solicitada.Conta.AccountType,
                Webservice = solicitada.Conta.Webservice,
                CodeContract = solicitada.Conta.CodeContract,
                DdaAtivado = solicitada.Conta.DdaActived,
                ClientKey = solicitada.Conta.ClientKey,
                ClientSecret = solicitada.Conta.ClientSecret,
                ClientId = solicitada.Conta.ClientId,
                RecipientNotification = solicitada.Conta.RecipientNotification,
                PagBBAtivado = solicitada.Conta.PagBBEnabled
            };

            var id = await _contaBancariaRepository.AddAsync(conta, cancellationToken);
            conta.Id = id;

            resultado.Add(new ContaResponseItem(
                conta.Id,
                conta.IdContaBancariaKodiak,
                conta.AccountHash,
                conta.BankCode,
                conta.Agency,
                conta.AgencyDigit,
                conta.AccountNumber,
                conta.AccountNumberDigit,
                conta.StatementAtivado,
                conta.OpenFinanceLink));
        }

        return Result.Ok(new CriarContaResponse(pagador.Id, resultado));
    }
}
