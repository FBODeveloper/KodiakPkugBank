using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class CriarPagadorUseCase
{
    private readonly IPlugBankApi _plugBankApi;
    private readonly IPagadorRepository _pagadorRepository;

    public CriarPagadorUseCase(IPlugBankApi plugBankApi, IPagadorRepository pagadorRepository)
    {
        _plugBankApi = plugBankApi;
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<PagadorResponse>> ExecuteAsync(
        CriarPagadorRequest request,
        PlugBankCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ChaveKodiakExtrato) || request.ChaveKodiakExtrato.Length > 1000)
            return Result.Fail<PagadorResponse>("ChaveKodiakExtrato é obrigatória e deve ter até 1000 caracteres.");

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.CpfCnpj))
            return Result.Fail<PagadorResponse>("Name e CpfCnpj são obrigatórios.");

        if (await _pagadorRepository.GetByCpfCnpjAsync(request.CpfCnpj, cancellationToken) is not null)
            return Result.Fail<PagadorResponse>($"Já existe pagador com o CPF/CNPJ {request.CpfCnpj}.", 409);

        var payerRequest = new CreatePayerRequest
        {
            Name = request.Name,
            Email = request.Email,
            CpfCnpj = request.CpfCnpj,
            DdaActived = request.DdaActived,
            StatementActived = request.StatementActived,
            Street = request.Street,
            Neighborhood = request.Neighborhood,
            AddressNumber = request.AddressNumber,
            AddressComplement = request.AddressComplement,
            City = request.City,
            State = request.State,
            Zipcode = request.Zipcode,
            Accounts = request.Accounts
        };

        CreatePayerResponse response;
        try
        {
            response = await _plugBankApi.CreatePayerAsync(payerRequest, credentials, cancellationToken);
        }
        catch (PlugBankException ex)
        {
            return Result.Fail<PagadorResponse>(ex.Message, ex.StatusCode);
        }

        var pagador = new Core.Entities.Pagador
        {
            Nome = response.Name ?? request.Name,
            Email = response.Email ?? request.Email,
            CpfCnpj = response.CpfCnpj ?? request.CpfCnpj,
            Logradouro = response.Street ?? request.Street,
            Bairro = response.Neighborhood ?? request.Neighborhood,
            NumeroEndereco = response.AddressNumber ?? request.AddressNumber,
            ComplementoEndereco = response.AddressComplement ?? request.AddressComplement,
            Cidade = response.City ?? request.City,
            Estado = response.State ?? request.State,
            Cep = response.Zipcode ?? request.Zipcode,
            Token = response.Token,
            StatementAtivado = response.StatementActived ?? request.StatementActived ?? false,
            ChaveKodiakExtrato = request.ChaveKodiakExtrato
        };

        var id = await _pagadorRepository.AddAsync(pagador, cancellationToken);
        pagador.Id = id;

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
