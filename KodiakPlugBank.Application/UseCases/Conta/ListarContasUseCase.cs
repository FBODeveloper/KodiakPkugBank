using KodiakPlugBank.Application.Common;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Application.UseCases.Conta;

public class ListarContasUseCase
{
    private readonly IContaBancariaRepository _contaBancariaRepository;

    public ListarContasUseCase(IContaBancariaRepository contaBancariaRepository)
    {
        _contaBancariaRepository = contaBancariaRepository;
    }

    public async Task<Result<IEnumerable<ContaResponseItem>>> ExecuteAsync(int idPagador, CancellationToken cancellationToken = default)
    {
        var contas = await _contaBancariaRepository.GetByPagadorIdAsync(idPagador, cancellationToken);
        return Result.Ok(contas.Select(Map));
    }

    internal static ContaResponseItem Map(ContaBancaria c) => new(
        c.Id,
        c.IdContaBancariaKodiak,
        c.AccountHash,
        c.BankCode,
        c.Agency,
        c.AgencyDigit,
        c.AccountNumber,
        c.AccountNumberDigit,
        c.StatementAtivado,
        c.OpenFinanceLink);
}
