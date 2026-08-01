using System.Security.Cryptography;
using System.Text;
using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class AutenticarApikeyFixaUseCase
{
    private readonly IApikeyFixaRepository _apikeyFixaRepository;

    public AutenticarApikeyFixaUseCase(IApikeyFixaRepository apikeyFixaRepository)
    {
        _apikeyFixaRepository = apikeyFixaRepository;
    }

    public async Task<Result> ExecuteAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return Result.Fail("Chave de acesso não informada.", 401);

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(apiKey))).ToLowerInvariant();
        var valida = await _apikeyFixaRepository.ExisteAtivaAsync(hash, cancellationToken);

        return valida ? Result.Ok() : Result.Fail("Chave de acesso inválida.", 401);
    }
}
