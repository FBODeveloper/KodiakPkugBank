using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.Payer;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class ListarPagadoresPlugBankUseCaseTests
{
    private readonly FakePlugBankApi _api = new();
    private readonly PlugBankCredentials _credentials = new() { CnpjSh = "cnpjsh", TokenSh = "tokensh" };

    private ListarPagadoresPlugBankUseCase BuildUseCase() => new(_api);

    [Fact]
    public async Task DeveListarPagadoresComSucesso()
    {
        var resultado = await BuildUseCase().ExecuteAsync(_credentials);

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
        Assert.Single(resultado.Value!.Payers!);
        Assert.Equal("Pagador Teste", resultado.Value.Payers![0].Name);
    }

    [Fact]
    public async Task DeveRepassarErroDaPlugBank()
    {
        _api.ListPayersHandler = _ => throw new PlugBankException(401, "Credenciais inválidas.");

        var resultado = await BuildUseCase().ExecuteAsync(_credentials);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }
}
