using Dapper;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Infrastructure.Data.Repositories;

public class PagadorRepository : IPagadorRepository
{
    private readonly DbConnectionFactory _factory;

    public PagadorRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    private const string SelectColumns = @"
        id, nome, email, cpf_cnpj, logradouro, bairro, numero_endereco,
        complemento_endereco, cidade, estado, cep, token, statement_ativado,
        chave_kodiak_extrato, criado_em";

    private const string InsertSql = @"
        INSERT INTO pagador (nome, email, cpf_cnpj, logradouro, bairro, numero_endereco,
            complemento_endereco, cidade, estado, cep, token, statement_ativado, chave_kodiak_extrato)
        VALUES (@Nome, @Email, @CpfCnpj, @Logradouro, @Bairro, @NumeroEndereco,
            @ComplementoEndereco, @Cidade, @Estado, @Cep, @Token, @StatementAtivado, @ChaveKodiakExtrato)
        RETURNING id;";

    private static Pagador Map(PagadorRow row) => new()
    {
        Id = row.Id,
        Nome = row.Nome,
        Email = row.Email,
        CpfCnpj = row.CpfCnpj,
        Logradouro = row.Logradouro ?? string.Empty,
        Bairro = row.Bairro ?? string.Empty,
        NumeroEndereco = row.NumeroEndereco,
        ComplementoEndereco = row.ComplementoEndereco,
        Cidade = row.Cidade ?? string.Empty,
        Estado = row.Estado ?? string.Empty,
        Cep = row.Cep ?? string.Empty,
        Token = row.Token,
        StatementAtivado = row.StatementAtivado,
        ChaveKodiakExtrato = row.ChaveKodiakExtrato
    };

    public async Task<Pagador?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync<PagadorRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM pagador WHERE id = @Id;
                """, new { Id = id }, cancellationToken: cancellationToken));
        return row is null ? null : Map(row);
    }

    public async Task<Pagador?> GetByCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync<PagadorRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM pagador WHERE cpf_cnpj = @CpfCnpj;
                """, new { CpfCnpj = cpfCnpj }, cancellationToken: cancellationToken));
        return row is null ? null : Map(row);
    }

    public async Task<IEnumerable<Pagador>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<PagadorRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM pagador ORDER BY id;
                """, cancellationToken: cancellationToken));
        return rows.Select(Map);
    }

    public async Task<int> AddAsync(Pagador pagador, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(InsertSql, pagador, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(Pagador pagador, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(new CommandDefinition("""
            UPDATE pagador SET
                nome = @Nome, email = @Email, cpf_cnpj = @CpfCnpj, logradouro = @Logradouro,
                bairro = @Bairro, numero_endereco = @NumeroEndereco, complemento_endereco = @ComplementoEndereco,
                cidade = @Cidade, estado = @Estado, cep = @Cep, token = @Token,
                statement_ativado = @StatementAtivado, chave_kodiak_extrato = @ChaveKodiakExtrato
            WHERE id = @Id;
            """, pagador, cancellationToken: cancellationToken));
    }
}

public class PagadorRow
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string CpfCnpj { get; set; } = string.Empty;
    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string? NumeroEndereco { get; set; }
    public string? ComplementoEndereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public string? Token { get; set; }
    public bool StatementAtivado { get; set; }
    public string ChaveKodiakExtrato { get; set; } = string.Empty;
}
