namespace KodiakPlugBank.Core.Interfaces.Repositories;

public interface IApikeyFixaRepository
{
    Task<bool> ExisteAtivaAsync(string hashSha256, CancellationToken cancellationToken = default);
}
