using KodiakPlugBank.Application.UseCases.Pagador;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Tests.Fakes;

namespace KodiakPlugBank.Tests.UseCases;

public class AutenticarPagadorUseCaseTests
{
    private readonly FakePagadorRepository _repo = new();

    private AutenticarPagadorUseCase BuildUseCase() => new(_repo);

    private void SeedPagador() => _repo.Data.Add(new Pagador
    {
        Id = 1,
        Nome = "Empresa Teste",
        CpfCnpj = "11111111000191",
        ChaveKodiakExtrato = "chave-valida"
    });

    [Fact]
    public async Task DeveAutenticarComChaveValida()
    {
        SeedPagador();

        var resultado = await BuildUseCase().ExecuteAsync("chave-valida");

        Assert.True(resultado.IsSuccess);
        Assert.NotNull(resultado.Value);
        Assert.Equal(1, resultado.Value!.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeveFalharQuandoChaveVazia(string? chave)
    {
        var resultado = await BuildUseCase().ExecuteAsync(chave!);

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }

    [Fact]
    public async Task DeveFalharQuandoChaveInvalida()
    {
        SeedPagador();

        var resultado = await BuildUseCase().ExecuteAsync("chave-inexistente");

        Assert.False(resultado.IsSuccess);
        Assert.Equal(401, resultado.StatusCode);
    }
}
