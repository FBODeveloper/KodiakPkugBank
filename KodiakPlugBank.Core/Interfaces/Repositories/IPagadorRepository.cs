using KodiakPlugBank.Core.Entities;

namespace KodiakPlugBank.Core.Interfaces.Repositories;

public interface IPagadorRepository
{
    Task<Pagador?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Pagador?> GetByCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default);
    Task<Pagador?> GetByChaveKodiakAsync(string chaveKodiak, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pagador>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> AddAsync(Pagador pagador, CancellationToken cancellationToken = default);
    Task UpdateAsync(Pagador pagador, CancellationToken cancellationToken = default);
}
