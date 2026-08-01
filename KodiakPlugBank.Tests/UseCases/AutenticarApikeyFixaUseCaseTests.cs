using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class AutenticarApikeyFixaUseCaseTests
{
    private const string ChaveValida = "kdk_live_8T9hV2qLmN7xP4sRwY5ZaBcDeFgHiJkLmNoPqRsTuVwXyZ123456";
    private const string HashValido = "d7944e9b351a320a612e659fc009e8d54dfc2be0b77d0b5f1b63d2a31c5b32a3";

    [Fact]
    public async Task DeveAutenticarComApikeyValida()
    {
        var repo = new FakeApikeyFixaRepository();
        repo.Hashes.Add(HashValido);

        var resultado = await new AutenticarApikeyFixaUseCase(repo).ExecuteAsync(ChaveValida);

        Assert.True(resultado.IsSuccess);
    }

    [Fact]
    public async Task DeveFalharQuandoHashNaoCadastrado()
    {
        var repo = new FakeApikeyFixaRepository();

        var resultado = await new AutenticarApikeyFixaUseCase(repo).ExecuteAsync(ChaveValida);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeveFalharQuandoChaveVazia(string? chave)
    {
        var repo = new FakeApikeyFixaRepository();

        var resultado = await new AutenticarApikeyFixaUseCase(repo).ExecuteAsync(chave!);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveGerarOByteHashDaChaveParaComparacao()
    {
        var repo = new FakeApikeyFixaRepository();
        repo.Hashes.Add(HashValido);

        var resultado = await new AutenticarApikeyFixaUseCase(repo).ExecuteAsync(ChaveValida);

        Assert.True(resultado.IsSuccess);
        Assert.Contains(HashValido, repo.Hashes);
    }
}
