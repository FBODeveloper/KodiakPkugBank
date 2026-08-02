using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using KodiakPlugBank.Core.Interfaces;
using KodiakPlugBank.Core.PlugBank.Account;
using KodiakPlugBank.Core.PlugBank.Common;
using KodiakPlugBank.Core.PlugBank.OpenFinance;
using KodiakPlugBank.Core.PlugBank.Payer;
using Microsoft.Extensions.Options;
using Options = KodiakPlugBank.Infrastructure.Options;

namespace KodiakPlugBank.Infrastructure.PlugBank;

public class PlugBankApiClient : IPlugBankApi
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public PlugBankApiClient(HttpClient http, IOptions<Options.PlugBankOptions> options)
    {
        _http = http;
        _baseUrl = options.Value.BaseUrl.TrimEnd('/');
    }

    public Task<CreatePayerResponse> CreatePayerAsync(CreatePayerRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<CreatePayerRequest, CreatePayerResponse>(
            HttpMethod.Post, "/api/v1/payer", request, credentials, cancellationToken);

    public Task<PayerConsultaResponse> GetPayerAsync(string payerCpfCnpj, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<object, PayerConsultaResponse>(
            HttpMethod.Get, "/api/v1/payer", null, WithPayer(credentials, payerCpfCnpj), cancellationToken);

    public Task<PayerListResponse> ListPayersAsync(PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<object, PayerListResponse>(
            HttpMethod.Get, "/api/v1/payer/list", null, credentials, cancellationToken);

    public Task<AtualizarPayerResponse> UpdatePayerAsync(CreatePayerRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<CreatePayerRequest, AtualizarPayerResponse>(
            HttpMethod.Put, "/api/v1/payer", request, credentials, cancellationToken);

    public Task<DesativarPayerResponse> DisablePayerAsync(string tokenPayer, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<object, DesativarPayerResponse>(
            HttpMethod.Delete, $"/api/v1/payer/{Uri.EscapeDataString(tokenPayer)}", null, credentials, cancellationToken);

    public Task<CreateAccountResponse> CreateAccountAsync(IReadOnlyList<CreateAccountItemRequest> request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<IReadOnlyList<CreateAccountItemRequest>, CreateAccountResponse>(
            HttpMethod.Post, "/api/v1/account", request, credentials, cancellationToken);

    public Task<CreateStatementResponse> CreateStatementAsync(CreateStatementRequest request, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<CreateStatementRequest, CreateStatementResponse>(
            HttpMethod.Post, "/api/v1/statement/openfinance", request, credentials, cancellationToken);

    public Task<StatementDocument> GetStatementAsync(string uniqueId, PlugBankCredentials credentials, CancellationToken cancellationToken = default)
        => SendAsync<object, StatementDocument>(
            HttpMethod.Get, $"/api/v1/statement/openfinance/{uniqueId}", null, credentials, cancellationToken);

    private static PlugBankCredentials WithPayer(PlugBankCredentials credentials, string payerCpfCnpj) => new()
    {
        CnpjSh = credentials.CnpjSh,
        TokenSh = credentials.TokenSh,
        PayerCpfCnpj = payerCpfCnpj
    };

    private async Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string path,
        TRequest? body,
        PlugBankCredentials credentials,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, $"{_baseUrl}{path}");
        request.Headers.Add("cnpjsh", credentials.CnpjSh);
        request.Headers.Add("tokensh", credentials.TokenSh);
        request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
        if (!string.IsNullOrWhiteSpace(credentials.PayerCpfCnpj))
            request.Headers.Add("payercpfcnpj", credentials.PayerCpfCnpj);

        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);

        using var response = await _http.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            PlugBankError? error = null;
            try
            {
                error = string.IsNullOrWhiteSpace(content)
                    ? null
                    : JsonSerializer.Deserialize<PlugBankError>(content, JsonOptions);
            }
            catch (JsonException)
            {
            }

            throw new PlugBankException(
                (int)response.StatusCode,
                error?.Message ?? $"Erro ao chamar a API da PlugBank ({(int)response.StatusCode}).",
                error);
        }

        return JsonSerializer.Deserialize<TResponse>(content, JsonOptions)
            ?? throw new PlugBankException(502, "A API da PlugBank retornou uma resposta vazia.");
    }
}
