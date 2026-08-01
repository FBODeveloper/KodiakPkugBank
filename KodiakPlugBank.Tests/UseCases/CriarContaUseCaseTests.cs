using KodiakPlugBank.Application.UseCases.Conta;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.PlugBank.Account;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class CriarContaUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly FakePagadorRepository _pagadorRepo = new();
    private readonly FakeContaBancariaRepository _contaRepo = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private CriarContaUseCase BuildUseCase() => new(_api, _pagadorRepo, _contaRepo);

    private static CriarContaRequest Request(int idPagador) => new(
        IdPagador: idPagador,
        Contas:
        [
            new CriarContaRequestItem(
                IdContaBancariaKodiak: 10,
                Conta: new CreateAccountItemRequest
                {
                    BankCode = "341",
                    Agency = "1111",
                    AccountNumber = "12345",
                    StatementActived = true
                })
        ]);

    private void SeedPagador()
    {
        _pagadorRepo.Data.Add(new Pagador
        {
            Id = 1,
            Nome = "Empresa Teste",
            CpfCnpj = "11111111000191",
            ChaveKodiakExtrato = "chave-teste"
        });
    }

    [Fact]
    public async Task DeveCriarContaComSucesso()
    {
        SeedPagador();

        var resultado = await BuildUseCase().ExecuteAsync(Request(1), _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.Single(resultado.Value!.Contas);
        Assert.Equal("hash-12345", resultado.Value.Contas[0].AccountHash);
        Assert.Equal(10, resultado.Value.Contas[0].IdContaBancariaKodiak);
        Assert.Single(_contaRepo.Data);
        Assert.Equal(1, _contaRepo.Data[0].IdPagador);
    }

    [Fact]
    public async Task DeveFalharQuandoPagadorNaoExiste()
    {
        var resultado = await BuildUseCase().ExecuteAsync(Request(99), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(404, resultado.StatusCode);
        Assert.Empty(_contaRepo.Data);
    }

    [Fact]
    public async Task DeveFalharQuandoListaDeContasVazia()
    {
        SeedPagador();

        var request = Request(1) with { Contas = [] };
        var resultado = await BuildUseCase().ExecuteAsync(request, _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(400, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveEnviarCpfCnpjDoPagadorNoHeader()
    {
        SeedPagador();
        string? payerCpfCnpjEnviado = null;

        _api.AccountHandler = (contas, credentials) =>
        {
            payerCpfCnpjEnviado = credentials.PayerCpfCnpj;
            return Task.FromResult(new KodiakPlugBank.Core.PlugBank.Account.CreateAccountResponse
            {
                Accounts = contas.Select(c => new CreateAccountResponseItem
                {
                    AccountHash = "hash-1",
                    BankCode = c.BankCode,
                    Agency = c.Agency,
                    AccountNumber = c.AccountNumber
                }).ToList()
            });
        };

        await BuildUseCase().ExecuteAsync(Request(1), _credentials);

        Assert.Equal("11111111000191", payerCpfCnpjEnviado);
    }
}
