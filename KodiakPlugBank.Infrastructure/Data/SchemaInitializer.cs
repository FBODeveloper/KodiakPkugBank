using System.Reflection;
using Dapper;
using KodiakPlugBank.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace KodiakPlugBank.Infrastructure.Data;

public class SchemaInitializer
{
    private readonly DbConnectionFactory _factory;
    private readonly string _connectionString;

    public SchemaInitializer(DbConnectionFactory factory, IOptions<DatabaseOptions> options)
    {
        _factory = factory;
        _connectionString = options.Value.ConnectionString;
    }

    public async Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        await EnsureDatabaseAsync(cancellationToken);

        const string resourceName = "KodiakPlugBank.Infrastructure.Scripts.schema.sql";
        var assembly = Assembly.GetExecutingAssembly();
        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Recurso embutido '{resourceName}' não encontrado.");
        using var reader = new StreamReader(stream);
        var script = await reader.ReadToEndAsync(cancellationToken);

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(new CommandDefinition(script, cancellationToken: cancellationToken));
    }

    private async Task EnsureDatabaseAsync(CancellationToken cancellationToken)
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        var database = builder.Database;
        if (string.IsNullOrWhiteSpace(database))
            return;

        builder.Database = "postgres";
        using var conn = new NpgsqlConnection(builder.ConnectionString);
        await conn.OpenAsync(cancellationToken);

        var exists = await conn.ExecuteScalarAsync<long>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM pg_database WHERE datname = @Database;",
                new { Database = database },
                cancellationToken: cancellationToken));

        if (exists == 0)
            await conn.ExecuteAsync(new CommandDefinition(
                $"CREATE DATABASE \"{database}\";",
                cancellationToken: cancellationToken));
    }
}
