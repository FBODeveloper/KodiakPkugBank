using KodiakPlugBank.Core.Entities;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.Interfaces.Repositories;
using KodiakPlugBank.Core.PlugBank.Account;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;
using KodiakPlugBank.Core.PlugBank.Payer;

namespace KodiakPlugBank.Tests.Fakes;

public class FakePagadorRepository : IPagadorRepository
{
    public List<Pagador> Data { get; } = new();
    public int NextId { get; set; } = 1;

    public Task<Pagador?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Data.FirstOrDefault(p => p.Id == id));

    public Task<Pagador?> GetByCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default)
        => Task.FromResult(Data.FirstOrDefault(p => p.CpfCnpj == cpfCnpj));

    public Task<IEnumerable<Pagador>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Pagador>>(Data.ToList());

    public Task<int> AddAsync(Pagador pagador, CancellationToken cancellationToken = default)
    {
        pagador.Id = NextId++;
        Data.Add(pagador);
        return Task.FromResult(pagador.Id);
    }

    public Task UpdateAsync(Pagador pagador, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public class FakeContaBancariaRepository : IContaBancariaRepository
{
    public List<ContaBancaria> Data { get; } = new();
    public int NextId { get; set; } = 1;

    public Task<ContaBancaria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => Task.FromResult(Data.FirstOrDefault(c => c.Id == id));

    public Task<ContaBancaria?> GetByIdContaBancariaKodiakAsync(int idContaBancariaKodiak, CancellationToken cancellationToken = default)
        => Task.FromResult(Data.FirstOrDefault(c => c.IdContaBancariaKodiak == idContaBancariaKodiak));

    public Task<ContaBancaria?> GetByAccountHashAsync(string accountHash, CancellationToken cancellationToken = default)
        => Task.FromResult(Data.FirstOrDefault(c => c.AccountHash == accountHash));

    public Task<IEnumerable<ContaBancaria>> GetByPagadorIdAsync(int idPagador, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<ContaBancaria>>(Data.Where(c => c.IdPagador == idPagador).ToList());

    public Task<int> AddAsync(ContaBancaria conta, CancellationToken cancellationToken = default)
    {
        conta.Id = NextId++;
        Data.Add(conta);
        return Task.FromResult(conta.Id);
    }

    public Task UpdateAsync(ContaBancaria conta, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public class FakeApikeyFixaRepository : IApikeyFixaRepository
{
    public HashSet<string> Hashes { get; } = new();

    public Task<bool> ExisteAtivaAsync(string hashSha256, CancellationToken cancellationToken = default)
        => Task.FromResult(Hashes.Contains(hashSha256));
}

public class FakePlugBankApi : IPlugBankApi
{
    public Func<CreatePayerRequest, PlugBankCredentials, Task<CreatePayerResponse>>? PayerHandler { get; set; }
    public Func<IReadOnlyList<CreateAccountItemRequest>, PlugBankCredentials, Task<CreateAccountResponse>>? AccountHandler { get; set; }
    public Func<CreateStatementRequest, PlugBankCredentials, Task<CreateStatementResponse>>? StatementHandler { get; set; }
    public Func<string, PlugBankCredentials, Task<StatementDocument>>? GetStatementHandler { get; set; }

    public Task<CreatePayerResponse> CreatePayerAsync(CreatePayerRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => PayerHandler?.Invoke(request, credentials) ?? Task.FromResult(new CreatePayerResponse
        {
            Name = request.Name,
            CpfCnpj = request.CpfCnpj,
            Token = "token-plugbank"
        });

    public Task<CreateAccountResponse> CreateAccountAsync(IReadOnlyList<CreateAccountItemRequest> request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => AccountHandler?.Invoke(request, credentials) ?? Task.FromResult(new CreateAccountResponse
        {
            Accounts = request.Select(r => new CreateAccountResponseItem
            {
                BankCode = r.BankCode,
                Agency = r.Agency,
                AccountNumber = r.AccountNumber,
                AccountHash = $"hash-{r.AccountNumber}",
                StatementActived = r.StatementActived
            }).ToList()
        });

    public Task<CreateStatementResponse> CreateStatementAsync(CreateStatementRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => StatementHandler?.Invoke(request, credentials) ?? Task.FromResult(new CreateStatementResponse
        {
            UniqueId = "unique-123",
            Type = "BANK"
        });

    public Task<StatementDocument> GetStatementAsync(string uniqueId, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => GetStatementHandler?.Invoke(uniqueId, credentials) ?? Task.FromResult(new StatementDocument
        {
            Statement = new StatementInfo { UniqueId = uniqueId, Status = "SUCCESS" }
        });
}
