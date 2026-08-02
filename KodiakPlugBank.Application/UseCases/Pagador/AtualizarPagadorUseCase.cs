using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class AtualizarPagadorUseCase
{
    private readonly IPlugBankApi _plugBankApi;
    private readonly IPagadorRepository _pagadorRepository;

    public AtualizarPagadorUseCase(IPlugBankApi plugBankApi, IPagadorRepository pagadorRepository)
    {
        _plugBankApi = plugBankApi;
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<PagadorResponse>> ExecuteAsync(
        string payerCpfCnpj,
        CreatePayerRequest request,
        PlugBankCredentials baseCredentials,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(payerCpfCnpj))
            return Result.Fail<PagadorResponse>("Header payercpfcnpj não informado.", 401);

        var pagador = await _pagadorRepository.GetByCpfCnpjAsync(payerCpfCnpj, cancellationToken);
        if (pagador is null)
            return Result.Fail<PagadorResponse>($"Pagador não encontrado para o payercpfcnpj {payerCpfCnpj}.", 404);

        if (!string.Equals(pagador.CpfCnpj, request.CpfCnpj, StringComparison.OrdinalIgnoreCase))
        {
            var conflitante = await _pagadorRepository.GetByCpfCnpjAsync(request.CpfCnpj, cancellationToken);
            if (conflitante is not null)
                return Result.Fail<PagadorResponse>($"Já existe pagador com o CPF/CNPJ {request.CpfCnpj}.", 409);
        }

        var credentials = new PlugBankCredentials
        {
            CnpjSh = baseCredentials.CnpjSh,
            TokenSh = baseCredentials.TokenSh,
            PayerCpfCnpj = pagador.CpfCnpj
        };

        AtualizarPayerResponse response;
        try
        {
            response = await _plugBankApi.UpdatePayerAsync(request, credentials, cancellationToken);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<PagadorResponse>(ex.Message, ex.StatusCode);
        }

        pagador.Nome = response.Name ?? request.Name ?? pagador.Nome;
        pagador.Email = response.Email ?? request.Email ?? pagador.Email;
        pagador.CpfCnpj = response.CpfCnpj ?? request.CpfCnpj ?? pagador.CpfCnpj;
        pagador.Logradouro = response.Street ?? request.Street ?? pagador.Logradouro;
        pagador.Bairro = response.Neighborhood ?? request.Neighborhood ?? pagador.Bairro;
        pagador.NumeroEndereco = response.AddressNumber ?? request.AddressNumber ?? pagador.NumeroEndereco;
        pagador.ComplementoEndereco = response.AddressComplement ?? request.AddressComplement ?? pagador.ComplementoEndereco;
        pagador.Cidade = response.City ?? request.City ?? pagador.Cidade;
        pagador.Estado = response.State ?? request.State ?? pagador.Estado;
        pagador.Cep = response.Zipcode ?? request.Zipcode ?? pagador.Cep;
        pagador.StatementAtivado = response.StatementActived ?? request.StatementActived ?? pagador.StatementAtivado;

        await _pagadorRepository.UpdateAsync(pagador, cancellationToken);

        return Result.Ok(new PagadorResponse(
            pagador.Id,
            pagador.Nome,
            pagador.Email,
            pagador.CpfCnpj,
            pagador.ChaveKodiakExtrato,
            pagador.Token,
            pagador.StatementAtivado));
    }
}
