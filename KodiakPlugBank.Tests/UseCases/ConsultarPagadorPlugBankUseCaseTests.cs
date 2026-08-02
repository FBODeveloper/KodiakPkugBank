using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class ConsultarPagadorPlugBankUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private ConsultarPagadorPlugBankUseCase BuildUseCase() => new(_api);

    [Fact]
    public async Task DeveConsultarPagadorComSucesso()
    {
        var resultado = await BuildUseCase().ExecuteAsync("11111111000191", _credentials);

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
        Assert.Equal("11111111000191", resultado.Value!.CpfCnpj);
        Assert.Equal("token-consulta", resultado.Value.Token);
    }

    [Fact]
    public async Task DeveFalharQuandoPayercpfcnpjNaoInformado()
    {
        var resultado = await BuildUseCase().ExecuteAsync("", _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveRepassarErroDaPlugBank()
    {
        _api.GetPayerHandler = (_, _) => throw new PlugBankException(422, "Pagador não encontrado.");

        var resultado = await BuildUseCase().ExecuteAsync("11111111000191", _credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(422, resultado.StatusCode);
    }
}
