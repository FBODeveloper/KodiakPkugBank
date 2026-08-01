using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class AutenticarPagadorUseCase
{
    private readonly IPagadorRepository _pagadorRepository;

    public AutenticarPagadorUseCase(IPagadorRepository pagadorRepository)
    {
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<Core.Entities.Pagador>> ExecuteAsync(string chaveKodiak, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(chaveKodiak))
            return Result.Fail<Core.Entities.Pagador>("Chave de acesso não informada.", 401);

        var pagador = await _pagadorRepository.GetByChaveKodiakAsync(chaveKodiak, cancellationToken);
        if (pagador is null)
            return Result.Fail<Core.Entities.Pagador>("Chave de acesso inválida.", 401);

        return Result.Ok(pagador);
    }
}
