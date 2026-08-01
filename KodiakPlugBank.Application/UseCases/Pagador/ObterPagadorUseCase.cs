using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class ObterPagadorUseCase
{
    private readonly IPagadorRepository _pagadorRepository;

    public ObterPagadorUseCase(IPagadorRepository pagadorRepository)
    {
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<PagadorResponse>> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        var pagador = await _pagadorRepository.GetByIdAsync(id, cancellationToken);
        if (pagador is null)
            return Result.Fail<PagadorResponse>("Pagador não encontrado.", 404);

        return Result.Ok(ListarPagadoresUseCase.Map(pagador));
    }
}
