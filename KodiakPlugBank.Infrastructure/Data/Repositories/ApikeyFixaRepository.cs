using Dapper;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Infrastructure.Data.Repositories;

public class ApikeyFixaRepository : IApikeyFixaRepository
{
    private readonly DbConnectionFactory _factory;

    public ApikeyFixaRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<bool> ExisteAtivaAsync(string hashSha256, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<bool>(new CommandDefinition("""
            SELECT EXISTS (SELECT 1 FROM apikey_fixa WHERE hash_sha256 = @Hash AND ativo);
            """, new { Hash = hashSha256 }, cancellationToken: cancellationToken));
    }
}
