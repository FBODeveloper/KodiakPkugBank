using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class DesativarPagadorUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly FakePagadorRepository _repo = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private DesativarPagadorUseCase BuildUseCase() => new(_api, _repo);

    private async Task<Pagador> SeedPagador()
    {
        var pagador = new Pagador
        {
            Nome = "Empresa Teste SA",
            CpfCnpj = "11111111000191",
            Token = "token-1",
            StatementAtivado = true,
            Ativo = true,
            ChaveKodiakExtrato = "chave-teste"
        };
        await _repo.AddAsync(pagador);
        return pagador;
    }

    [Fact]
    public async Task DeveDesativarPagadorComSucesso()
    {
        var pagador = await SeedPagador();
        string? tokenEnviado = null;
        string? payerEnviado = null;

        _api.DisablePayerHandler = (token, credentials) =>
        {
            tokenEnviado = token;
            payerEnviado = credentials.PayerCpfCnpj;
            return Task.FromResult(new DesativarPayerResponse
            {
                Active = false,
                Message = "Pagador desativado com sucesso",
                Payer = new DesativarPayerInfo { Name = pagador.Nome }
            });
        };

        var resultado = await BuildUseCase().ExecuteAsync(pagador.Token!, pagador.CpfCnpj, _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.False(resultado.Value!.Active);
        Assert.Equal("Pagador desativado com sucesso", resultado.Value.Message);
        Assert.Equal("token-1", tokenEnviado);
        Assert.Equal(pagador.CpfCnpj, payerEnviado);
        Assert.False(_repo.Data[0].Ativo);
    }

    [Fact]
    public async Task DeveFalharQuandoPayercpfcnpjNaoInformado()
    {
        var resultado = await BuildUseCase().ExecuteAsync("token-1", "", _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
        Assert.True(_repo.Data.Count == 0);
    }

    [Fact]
    public async Task DeveFalharQuandoPagadorNaoEncontrado()
    {
        var resultado = await BuildUseCase().ExecuteAsync("token-x", "99999999000191", _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(404, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveRepassarErroDaPlugBank()
    {
        var pagador = await SeedPagador();
        _api.DisablePayerHandler = (_, _) => throw new PlugBankException(422, "Não foi possível desativar.");

        var resultado = await BuildUseCase().ExecuteAsync(pagador.Token!, pagador.CpfCnpj, _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(422, resultado.StatusCode);
        Assert.True(_repo.Data[0].Ativo);
    }
}
