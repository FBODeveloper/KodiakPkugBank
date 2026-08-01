using System.Net;
using System.Text;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;
using KodiakPlugBank.Core.PlugBank.Payer;
using KodiakPlugBank.Infrastructure.Options;
using KodiakPlugBank.Infrastructure.PlugBank;
using Microsoft.Extensions.Options;

namespace KodiakPlugBank.Tests.PlugBank;

public class PlugBankApiClientTests
{
    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
        public List<HttpRequestMessage> Requests { get; } = new();

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(_responder(request));
        }
    }

    private static PlugBankApiClient BuildClient(StubHttpMessageHandler handler)
    {
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://api.teste.com") };
        return new PlugBankApiClient(http, Options.Create(new PlugBankOptions { BaseUrl = "https://api.teste.com" }));
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json) => new(status)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    [Fact]
    public async Task DeveEnviarCredenciaisNosHeaders()
    {
        var handler = new StubHttpMessageHandler(_ => JsonResponse(HttpStatusCode.Created, "{}"));
        var client = BuildClient(handler);

        await client.CreatePayerAsync(new CreatePayerRequest { Name = "Teste" }, new PlugBankCredentials
        {
            CnpjSh = "cnpjsh-test",
            TokenSh = "tokensh-test",
            PayerCpfCnpj = "11111111000191"
        });

        var request = handler.Requests.Single();
        Assert.Equal("cnpjsh-test", request.Headers.GetValues("cnpjsh").Single());
        Assert.Equal("tokensh-test", request.Headers.GetValues("tokensh").Single());
        Assert.Equal("11111111000191", request.Headers.GetValues("payercpfcnpj").Single());
        Assert.Equal("https://api.teste.com/api/v1/payer", request.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task DeveDeserializarExtrato()
    {
        const string json = """
        {
          "statement": {
            "uniqueId": "uqjpnm1kDj0ab0",
            "dateStart": "2024-10-20",
            "dateEnd": "2025-09-17",
            "bankCode": "208",
            "totalTransactions": "2",
            "origin": "OPENFINANCE",
            "accountHash": "gDyriq_q39",
            "status": "SUCCESS",
            "reason": "Extrato realizado com sucesso",
            "type": "BANK"
          },
          "transaction": {
            "credit": [
              {
                "transactionId": "048ffd42-4ae0-4344-9c51-c60a2b406dea",
                "transactionType": "credit",
                "code": "DIGITALSERVICES",
                "amount": "18.99",
                "date": "2024-10-20",
                "sequence": 1,
                "description": "DL*GOOGLEYouTube",
                "participantPayer": {
                  "name": "John Doe",
                  "documentNumber": { "type": "cpf", "value": "12345678900" }
                }
              }
            ],
            "debit": []
          },
          "balance": {
            "inicial": { "date": "2024-10-20", "balance": "100.00" },
            "final": { "date": "2025-09-17", "balance": "18.99" }
          }
        }
        """;
        var handler = new StubHttpMessageHandler(_ => JsonResponse(HttpStatusCode.OK, json));
        var client = BuildClient(handler);

        var doc = await client.GetStatementAsync("uqjpnm1kDj0ab0", new PlugBankCredentials());

        Assert.NotNull(doc.Statement);
        Assert.Equal("SUCCESS", doc.Statement!.Status);
        Assert.Equal("BANK", doc.Statement.Type);
        Assert.Single(doc.Transaction!.Credit!);
        Assert.Empty(doc.Transaction.Debit!);
        Assert.Equal("18.99", doc.Transaction.Credit![0].Amount);
        Assert.Equal("cpf", doc.Transaction.Credit[0].ParticipantPayer!.DocumentNumber!.Type);
        Assert.Equal("100.00", doc.Balance!.Inicial!.Balance);
        Assert.Equal("18.99", doc.Balance.Final!.Balance);
    }

    [Fact]
    public async Task DeveLancarPlugBankExceptionComErroDesserializado()
    {
        const string json = """
        {
          "code": 422,
          "message": "Unprocessable Entity",
          "errors": [ { "message": "Campo tokensh é obrigatório", "internalCode": 4001 } ]
        }
        """;
        var handler = new StubHttpMessageHandler(_ => JsonResponse(HttpStatusCode.UnprocessableEntity, json));
        var client = BuildClient(handler);

        var ex = await Assert.ThrowsAsync<PlugBankException>(() =>
            client.CreatePayerAsync(new CreatePayerRequest(), new PlugBankCredentials()));

        Assert.Equal(422, ex.StatusCode);
        Assert.Equal("Unprocessable Entity", ex.Error!.Message);
        Assert.Equal("Campo tokensh é obrigatório", ex.Error.Errors![0].Message);
        Assert.Equal(4001, ex.Error.Errors[0].InternalCode);
    }
}
