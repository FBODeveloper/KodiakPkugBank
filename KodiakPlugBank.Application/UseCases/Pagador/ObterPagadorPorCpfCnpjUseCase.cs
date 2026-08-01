using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Application.UseCases.Pagador;

public class ObterPagadorPorCpfCnpjUseCase
{
    private readonly IPagadorRepository _pagadorRepository;

    public ObterPagadorPorCpfCnpjUseCase(IPagadorRepository pagadorRepository)
    {
        _pagadorRepository = pagadorRepository;
    }

    public async Task<Result<Core.Entities.Pagador>> ExecuteAsync(
        string cpfCnpj,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpfCnpj))
            return Result.Fail<Core.Entities.Pagador>("Header payercpfcnpj não informado.", 401);

        var pagador = await _pagadorRepository.GetByCpfCnpjAsync(cpfCnpj, cancellationToken);
        if (pagador is null)
            return Result.Fail<Core.Entities.Pagador>($"Pagador não encontrado para o payercpfcnpj {cpfCnpj}.", 404);

        return Result.Ok(pagador);
    }
}
