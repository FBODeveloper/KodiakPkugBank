using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class ListarPagadoresUseCase
{
    private readonly IPagadorRepository _pagadorRepository;

    public ListarPagadoresUseCase(IPagadorRepository pagadorRepository)
    {
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<IEnumerable<PagadorResponse>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var pagadores = await _pagadorRepository.GetAllAsync(cancellationToken);
        return Result.Ok(pagadores.Select(Map));
    }

    internal static PagadorResponse Map(Core.Entities.Pagador p) => new(
        p.Id,
        p.Nome,
        p.Email,
        p.CpfCnpj,
        p.ChaveKodiakExtrato,
        p.Token,
        p.StatementAtivado);
}
