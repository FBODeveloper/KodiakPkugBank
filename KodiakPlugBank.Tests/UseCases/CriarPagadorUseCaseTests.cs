using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class CriarPagadorUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly FakePagadorRepository _repo = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private CriarPagadorUseCase BuildUseCase() => new(_api, _repo);

    private static CriarPagadorRequest Request(string? chave = "chave-teste") => new(
        Name: "Empresa Teste SA",
        Email: "teste@teste.com",
        CpfCnpj: "11111111000191",
        DdaActived: false,
        StatementActived: true,
        Street: "Rua A",
        Neighborhood: "Centro",
        AddressNumber: "100",
        AddressComplement: null,
        City: "Maringa",
        State: "PR",
        Zipcode: "87020000",
        Accounts: null,
        ChaveKodiakExtrato: chave!);

    [Fact]
    public async Task DeveCriarPagadorComSucesso()
    {
        var resultado = await BuildUseCase().ExecuteAsync(Request(), _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
        Assert.Equal("Empresa Teste SA", resultado.Value!.Nome);
        Assert.Equal("chave-teste", resultado.Value.ChaveKodiakExtrato);
        Assert.Equal("token-plugbank", resultado.Value.Token);
        Assert.Single(_repo.Data);
        Assert.Equal(1, _repo.Data[0].Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeveFalharQuandoChaveKodiakVazia(string? chave)
    {
        var resultado = await BuildUseCase().ExecuteAsync(Request(chave), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(400, resultado.StatusCode);
        Assert.Empty(_repo.Data);
    }

    [Fact]
    public async Task DeveFalharQuandoCpfCnpjJaCadastrado()
    {
        var useCase = BuildUseCase();
        await useCase.ExecuteAsync(Request(), _credentials);

        var segundo = await useCase.ExecuteAsync(Request("outra-chave"), _credentials);

        Assert.False(segundo.IsSuccess);
        Assert.Equal(409, segundo.StatusCode);
        Assert.Single(_repo.Data);
    }

    [Fact]
    public async Task DeveRepassarErroDaPlugBank()
    {
        _api.PayerHandler = (_, _) => throw new PlugBankException(422, "Campo obrigatório ausente.");

        var resultado = await BuildUseCase().ExecuteAsync(Request(), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(422, resultado.StatusCode);
        Assert.Empty(_repo.Data);
    }

    [Fact]
    public async Task DeveSalvarDadosRetornadosPelaPlugBank()
    {
        _api.PayerHandler = (_, _) => Task.FromResult(new KodiakPlugBank.Core.PlugBank.Payer.CreatePayerResponse
        {
            Name = "Nome Da API",
            CpfCnpj = "99999999000191",
            Token = "token-api",
            StatementActived = true
        });

        var resultado = await BuildUseCase().ExecuteAsync(Request(), _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.Equal("Nome Da API", _repo.Data[0].Nome);
        Assert.Equal("99999999000191", _repo.Data[0].CpfCnpj);
        Assert.Equal("token-api", _repo.Data[0].Token);
        Assert.True(_repo.Data[0].StatementAtivado);
    }
}
