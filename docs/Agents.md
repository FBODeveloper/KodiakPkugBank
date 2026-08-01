##COMUNICAÇÃO
- ** Sempre se comunicar em português Brasil.
- ** Criação e manipulação de banco de dados está liberada para criação e manutenção de estrutura, para manipulação de dados solicitar permissão sempre.
- ** Após qualquer tarefa ser concluída registrar em contexto para comunicação futura.
- ** Utilize seu melhor para potenciar e otimizar códigos e escritas.

##CONTEXTO DE DESENVOLVIMENTO (2026-08-01)
### Estrutura criada
- Solução `KodiakPlugBank.slnx` (.NET 10) com 5 projetos:
  - `KodiakPlugBank.Core` — entidades (Pagador, ContaBancaria), contratos PlugBank (Payer, Account, OpenFinance), enums e interfaces (IPagadorRepository, IContaBancariaRepository, IPlugBankApi).
  - `KodiakPlugBank.Application` — casos de uso (CriarPagador, ListarPagadores, ObterPagador, AutenticarPagador, CriarConta, ListarContas, CriarExtrato, ObterExtrato) + Result/Result<T>.
  - `KodiakPlugBank.Infrastructure` — Dapper + Npgsql (repositórios, DbConnectionFactory, SchemaInitializer), cliente HTTP PlugBankApiClient, options (DatabaseOptions, PlugBankOptions), DI (AddInfrastructure).
  - `KodiakPlugBank.Api` — Minimal API: middleware de autenticação via apikey (header `X-Api-Key`), endpoints `/api/v1/payer`, `/api/v1/account`, `/api/v1/statement/openfinance`, schema automático na inicialização.
  - `KodiakPlugBank.Tests` — xUnit (26 testes): casos de uso com fakes, PlugBankApiClient (desserialização, headers, erros) e mapeamento de variáveis de ambiente.
- Pacotes: Dapper 2.1.79, Npgsql 10.0.3, Microsoft.OpenApi 2.7.5 (correção CVE-2026-49451), Swashbuckle.AspNetCore 10.2.3.
- Documentação de uso via Swagger UI (somente em desenvolvimento): `/swagger` (UI) e `/swagger/v1/swagger.json` (JSON gerado pelo Swashbuckle). Substituiu o `MapOpenApi` nativo (`/openapi`). O middleware libera `/swagger` e `/openapi` como públicos; demais rotas exigem `X-Api-Key`.

### Segurança (proteção contra DDoS) — adicionado em 2026-08-01
- Rate limiting nativo (`Microsoft.AspNetCore.RateLimiting`, janela deslizante, partição por IP): política global 100 req/10s e política `bootstrap` (POST /api/v1/payer) 10 req/60s. Resposta 429 com `Retry-After`. Config em `appsettings.json` → `RateLimiting` (classe `KodiakPlugBank.Api/Security/RateLimitingExtensions.cs`).
- Forwarded Headers habilitado (`X-Forwarded-For` + `X-Forwarded-Proto`) para enxergar o IP real do cliente atrás de proxy/CDN. `KnownProxies`/`KnownNetworks` configuráveis em `appsettings.json` → `ForwardedHeaders` (usar faixas de IP oficiais do proxy em produção; senão o header pode ser falsificado).
- Limites do Kestrel em `appsettings.json` → `Kestrel:Limits`: `MaxRequestBodySize` 10MB, `MaxConcurrentConnections` 500, `MaxConcurrentUpgradedConnections` 100, `KeepAliveTimeout` 30s, `RequestHeadersTimeout` 5s, `MinRequestBodyDataRate`/`MinResponseDataRate` 240B/s (proteção contra slowloris).
- Middleware `SecurityHeadersMiddleware` adiciona `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: no-referrer` em todas as respostas.
- Ordem do pipeline: `UseForwardedHeaders → SecurityHeaders → Swagger(dev) → UseRateLimiter → ApiKeyMiddleware → Endpoints`.
- Endpoints e rotas: `POST /api/v1/payer` usa `RequireRateLimiting("bootstrap")`.
- Documentação completa: `docs/ConfiguracaoSeguranca.md`.

### Banco de dados
- Postgres local: database `kodiak_plugbank` criado automaticamente (connection: localhost:5432, postgres / 123!asd).
- Tabelas `pagador` e `conta_bancaria` criadas via `KodiakPlugBank.Infrastructure/Scripts/schema.sql` (EmbeddedResource).

### Decisões e convenções
- Autenticação exclusiva via apikey. Header `X-Api-Key` = `ChaveKodiakExtrato` do pagador. Endpoint `POST /api/v1/payer` (bootstrap) exige a chave mestre `Security:MasterApiKey` no mesmo header.
- Endpoints espelham header/body da API PlugBank (cnpjsh, tokensh, payercpfcnpj, campos do body).
- Contas NÃO são persistidas junto ao pagador: a associação é feita posteriormente via endpoint de conta (conforme doc do projeto).
- Configuração em `appsettings.json`: `Database`, `PlugBank` (BaseUrl staging por padrão, CnpjSh, TokenSh), `Security:MasterApiKey` (vazia por padrão).
- Nomenclatura: entidades/repositórios em pt-BR; DTOs de integração PlugBank em inglês espelhando a API.

### Como executar
- `dotnet run --project KodiakPlugBank.Api` (inicia na porta do launchSettings; schema aplicado automaticamente).
- `dotnet test KodiakPlugBank.slnx`

### Autenticação na TecnoSped PlugBank (variáveis de ambiente)
- `cnpjsh` é lido da variável de ambiente `KODIAK_PLUGBANK_SH`.
- `tokensh` é lido da variável de ambiente `KODIAK_PLUGBANK`.
- Mapeamento implementado em `KodiakPlugBank.Api/ConfigurationExtensions.cs` (precedência sobre appsettings).
- `appsettings.json` mantém apenas `PlugBank:BaseUrl` (staging por padrão).
- 3 testes cobrem o mapeamento/precedência em `Tests/Api/ConfigurationExtensionsTests.cs`.

### Documentos úteis
- `docs/ConfiguracaoBancoDados.md` — guia para preparar o banco de dados em **outras máquinas**
  (pré-requisitos, instalação automática via API ou manual via `psql`, connection string por variável
  de ambiente `Database__ConnectionString`, verificação e troubleshooting).
- `docs/ProjetoKodiakPlugBank.md` — visão geral e funções do projeto.
- `docs/IntegracaoPlugBankTecnoSped.md` — rotas da API PlugBank e variáveis de ambiente de autenticação.
- `docs/ConfiguracaoSeguranca.md` — proteção contra DDoS e configuração de produção (rate limiting, proxies, Kestrel).

### Pendências / próximos passos
- Configurar CnpjSh/TokenSh reais da TecnoSpeed e chave mestre antes de produção.
- Definir a API base (staging vs produção) conforme ambiente.
