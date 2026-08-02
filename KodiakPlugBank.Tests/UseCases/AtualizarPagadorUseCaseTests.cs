using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class AtualizarPagadorUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly FakePagadorRepository _repo = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private AtualizarPagadorUseCase BuildUseCase() => new(_api, _repo);

    private async Task<Pagador> SeedPagador()
    {
        var pagador = new Pagador
        {
            Nome = "Empresa Teste SA",
            Email = "teste@teste.com",
            CpfCnpj = "11111111000191",
            Logradouro = "Rua A",
            Bairro = "Centro",
            NumeroEndereco = "100",
            Cidade = "Maringa",
            Estado = "PR",
            Cep = "87020000",
            Token = "token-1",
            StatementAtivado = true,
            Ativo = true,
            ChaveKodiakExtrato = "chave-teste"
        };
        await _repo.AddAsync(pagador);
        return pagador;
    }

    private static CreatePayerRequest Request(string? nome = "Empresa Atualizada SA") => new()
    {
        Name = nome!,
        Email = "novo@teste.com",
        CpfCnpj = "11111111000191",
        Street = "Rua B",
        Neighborhood = "Jardim",
        AddressNumber = "200",
        City = "Londrina",
        State = "PR",
        Zipcode = "86020000",
        StatementActived = false
    };

    [Fact]
    public async Task DeveAtualizarPagadorComSucesso()
    {
        var pagador = await SeedPagador();
        string? payerEnviado = null;

        _api.UpdatePayerHandler = (_, credentials) =>
        {
            payerEnviado = credentials.PayerCpfCnpj;
            return Task.FromResult(new AtualizarPayerResponse
            {
                Name = "Nome Da API",
                Email = "api@teste.com",
                CpfCnpj = "11111111000191",
                StatementActived = true
            });
        };

        var resultado = await BuildUseCase().ExecuteAsync(pagador.CpfCnpj, Request(), _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.Equal("Nome Da API", resultado.Value!.Nome);
        Assert.Equal("Nome Da API", _repo.Data[0].Nome);
        Assert.Equal("api@teste.com", _repo.Data[0].Email);
        Assert.True(_repo.Data[0].StatementAtivado);
        Assert.Equal("Rua B", _repo.Data[0].Logradouro);
        Assert.Equal("token-1", _repo.Data[0].Token);
        Assert.Equal(pagador.CpfCnpj, payerEnviado);
    }

    [Fact]
    public async Task DeveFalharQuandoPayercpfcnpjNaoInformado()
    {
        var resultado = await BuildUseCase().ExecuteAsync("", Request(), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveFalharQuandoPagadorNaoEncontrado()
    {
        var resultado = await BuildUseCase().ExecuteAsync("99999999000191", Request(), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(404, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveFalharQuandoCpfCnpjNovoJaCadastrado()
    {
        await SeedPagador();
        await _repo.AddAsync(new Pagador { Nome = "Outro", CpfCnpj = "22222222000191" });

        var request = Request();
        request.CpfCnpj = "22222222000191";

        var resultado = await BuildUseCase().ExecuteAsync(
            "11111111000191",
            request,
            _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(409, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveRepassarErroDaPlugBank()
    {
        var pagador = await SeedPagador();
        _api.UpdatePayerHandler = (_, _) => throw new PlugBankException(422, "Campo inválido.");

        var resultado = await BuildUseCase().ExecuteAsync(pagador.CpfCnpj, Request(), _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(422, resultado.StatusCode);
        Assert.Equal("Campo inválido.", resultado.Error);
    }
}
