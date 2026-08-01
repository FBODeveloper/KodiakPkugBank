using KodiakPlugBank.Application.UseCases.Extrato;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class CriarExtratoUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly FakePagadorRepository _pagadorRepo = new();
    private readonly FakeContaBancariaRepository _contaRepo = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private CriarExtratoUseCase BuildUseCase() => new(_api, _pagadorRepo, _contaRepo);

    private void SeedPagador() => _pagadorRepo.Data.Add(new Pagador
    {
        Id = 1,
        Nome = "Empresa Teste",
        CpfCnpj = "11111111000191",
        ChaveKodiakExtrato = "chave-teste"
    });

    private static CriarExtratoRequest Request(string? accountHash = null) => new(
        IdPagador: 1,
        AccountHash: accountHash,
        Today: null,
        DateStart: "2025-01-01",
        DateEnd: "2025-01-31",
        StatementType: null,
        CardNumber: null);

    [Fact]
    public async Task DeveCriarExtratoComSucesso()
    {
        SeedPagador();

        var resultado = await BuildUseCase().ExecuteAsync(Request(), _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.Equal("unique-123", resultado.Value!.UniqueId);
    }

    [Fact]
    public async Task DeveFalharQuandoPagadorNaoExiste()
    {
        var resultado = await BuildUseCase().ExecuteAsync(Request(), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(404, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveFalharQuandoContaNaoPertenceAoPagador()
    {
        SeedPagador();
        _contaRepo.Data.Add(new ContaBancaria
        {
            Id = 1,
            IdPagador = 2,
            AccountHash = "hash-outro-pagador",
            BankCode = "341",
            Agency = "1",
            AccountNumber = "1"
        });

        var resultado = await BuildUseCase().ExecuteAsync(Request("hash-outro-pagador"), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(403, resultado.StatusCode);
    }

    [Fact]
    public async Task DevePermitirContaDoProprioPagador()
    {
        SeedPagador();
        _contaRepo.Data.Add(new ContaBancaria
        {
            Id = 1,
            IdPagador = 1,
            AccountHash = "hash-propria",
            BankCode = "341",
            Agency = "1",
            AccountNumber = "1"
        });

        var resultado = await BuildUseCase().ExecuteAsync(Request("hash-propria"), _credentials);

        Assert.True(resultado.IsSuccess);
    }
}
