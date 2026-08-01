using Dapper;
using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.Interfaces.Repositories;

namespace KodiakPlugBank.Infrastructure.Data.Repositories;

public class ContaBancariaRepository : IContaBancariaRepository
{
    private readonly DbConnectionFactory _factory;

    public ContaBancariaRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    private const string SelectColumns = @"
        id, id_pagador, id_conta_bancaria_kodiak, account_hash, bank_code, agency,
        agency_digit, account_number, account_number_digit, account_dac, account_type,
        account_payment, governmental_resource, convenio_agency, convenio_number,
        remessa_sequential, webservice, code_contract, dda_ativado, client_key,
        client_secret, client_id, recipient_notification, statement_ativado,
        pagbb_ativado, openfinance_link, criado_em";

    private const string InsertSql = @"
        INSERT INTO conta_bancaria (id_pagador, id_conta_bancaria_kodiak, account_hash, bank_code, agency,
            agency_digit, account_number, account_number_digit, account_dac, account_type,
            account_payment, governmental_resource, convenio_agency, convenio_number,
            remessa_sequential, webservice, code_contract, dda_ativado, client_key,
            client_secret, client_id, recipient_notification, statement_ativado,
            pagbb_ativado, openfinance_link)
        VALUES (@IdPagador, @IdContaBancariaKodiak, @AccountHash, @BankCode, @Agency,
            @AgencyDigit, @AccountNumber, @AccountNumberDigit, @AccountDac, @AccountType,
            @AccountPayment, @GovernmentalResource, @ConvenioAgency, @ConvenioNumber,
            @RemessaSequential, @Webservice, @CodeContract, @DdaAtivado, @ClientKey,
            @ClientSecret, @ClientId, @RecipientNotification, @StatementAtivado,
            @PagBBAtivado, @OpenFinanceLink)
        RETURNING id;";

    private static ContaBancaria Map(ContaBancariaRow row) => new()
    {
        Id = row.Id,
        IdPagador = row.IdPagador,
        IdContaBancariaKodiak = row.IdContaBancariaKodiak,
        AccountHash = row.AccountHash,
        BankCode = row.BankCode ?? string.Empty,
        Agency = row.Agency ?? string.Empty,
        AgencyDigit = row.AgencyDigit,
        AccountNumber = row.AccountNumber ?? string.Empty,
        AccountNumberDigit = row.AccountNumberDigit,
        AccountDac = row.AccountDac,
        AccountType = row.AccountType,
        AccountPayment = row.AccountPayment,
        GovernmentalResource = row.GovernmentalResource,
        ConvenioAgency = row.ConvenioAgency,
        ConvenioNumber = row.ConvenioNumber,
        RemessaSequential = row.RemessaSequential,
        Webservice = row.Webservice,
        CodeContract = row.CodeContract,
        DdaAtivado = row.DdaAtivado,
        ClientKey = row.ClientKey,
        ClientSecret = row.ClientSecret,
        ClientId = row.ClientId,
        RecipientNotification = row.RecipientNotification,
        StatementAtivado = row.StatementAtivado,
        PagBBAtivado = row.PagBBAtivado,
        OpenFinanceLink = row.OpenFinanceLink
    };

    public async Task<ContaBancaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync<ContaBancariaRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM conta_bancaria WHERE id = @Id;
                """, new { Id = id }, cancellationToken: cancellationToken));
        return row is null ? null : Map(row);
    }

    public async Task<ContaBancaria?> GetByIdContaBancariaKodiakAsync(int idContaBancariaKodiak, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync<ContaBancariaRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM conta_bancaria WHERE id_conta_bancaria_kodiak = @Id;
                """, new { Id = idContaBancariaKodiak }, cancellationToken: cancellationToken));
        return row is null ? null : Map(row);
    }

    public async Task<ContaBancaria?> GetByAccountHashAsync(string accountHash, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var row = await conn.QueryFirstOrDefaultAsync<ContaBancariaRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM conta_bancaria WHERE account_hash = @AccountHash;
                """, new { AccountHash = accountHash }, cancellationToken: cancellationToken));
        return row is null ? null : Map(row);
    }

    public async Task<IEnumerable<ContaBancaria>> GetByPagadorIdAsync(int idPagador, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<ContaBancariaRow>(
            new CommandDefinition($"""
                SELECT {SelectColumns} FROM conta_bancaria WHERE id_pagador = @IdPagador ORDER BY id;
                """, new { IdPagador = idPagador }, cancellationToken: cancellationToken));
        return rows.Select(Map);
    }

    public async Task<int> AddAsync(ContaBancaria conta, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(InsertSql, conta, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(ContaBancaria conta, CancellationToken cancellationToken = default)
    {
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(new CommandDefinition("""
            UPDATE conta_bancaria SET
                account_hash = @AccountHash, bank_code = @BankCode, agency = @Agency,
                agency_digit = @AgencyDigit, account_number = @AccountNumber,
                account_number_digit = @AccountNumberDigit, account_dac = @AccountDac,
                account_type = @AccountType, account_payment = @AccountPayment,
                governmental_resource = @GovernmentalResource, convenio_agency = @ConvenioAgency,
                convenio_number = @ConvenioNumber, remessa_sequential = @RemessaSequential,
                webservice = @Webservice, code_contract = @CodeContract, dda_ativado = @DdaAtivado,
                client_key = @ClientKey, client_secret = @ClientSecret, client_id = @ClientId,
                recipient_notification = @RecipientNotification, statement_ativado = @StatementAtivado,
                pagbb_ativado = @PagBBAtivado, openfinance_link = @OpenFinanceLink
            WHERE id = @Id;
            """, conta, cancellationToken: cancellationToken));
    }
}

public class ContaBancariaRow
{
    public int Id { get; set; }
    public int IdPagador { get; set; }
    public int IdContaBancariaKodiak { get; set; }
    public string? AccountHash { get; set; }
    public string? BankCode { get; set; }
    public string? Agency { get; set; }
    public string? AgencyDigit { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountNumberDigit { get; set; }
    public string? AccountDac { get; set; }
    public string? AccountType { get; set; }
    public bool AccountPayment { get; set; }
    public bool GovernmentalResource { get; set; }
    public string? ConvenioAgency { get; set; }
    public string? ConvenioNumber { get; set; }
    public long? RemessaSequential { get; set; }
    public bool Webservice { get; set; }
    public string? CodeContract { get; set; }
    public bool DdaAtivado { get; set; }
    public string? ClientKey { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientId { get; set; }
    public bool RecipientNotification { get; set; }
    public bool StatementAtivado { get; set; }
    public bool PagBBAtivado { get; set; }
    public string? OpenFinanceLink { get; set; }
}
