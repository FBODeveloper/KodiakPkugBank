using KodiakPlugBank.Core.Entities;

namespace KodiakPlugBank.Core.Interfaces.Repositories;

public interface IContaBancariaRepository
{
    Task<ContaBancaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ContaBancaria?> GetByIdContaBancariaKodiakAsync(int idContaBancariaKodiak, CancellationToken cancellationToken = default);
    Task<ContaBancaria?> GetByAccountHashAsync(string accountHash, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContaBancaria>> GetByPagadorIdAsync(int idPagador, CancellationToken cancellationToken = default);
    Task<int> AddAsync(ContaBancaria conta, CancellationToken cancellationToken = default);
    Task UpdateAsync(ContaBancaria conta, CancellationToken cancellationToken = default);
}
