##COMUNICAÇÃO
- ** Sempre se comunicar em português Brasil.
- ** Criação e manipulação de banco de dados está liberada para criação e manutenção de estrutura, para manipulação de dados solicitar permissão sempre.
- ** Após qualquer tarefa ser concluída registrar em contexto para comunicação futura.
- ** Utilize seu melhor para potenciar e otimizar códigos e escritas.

##CONTEXTO DE DESENVOLVIMENTO (2026-08-01)
### Estrutura criada
- Solução `KodiakPlugBank.slnx` (.NET 10) com 5 projetos:
  - `KodiakPlugBank.Core` — entidades (Pagador, ContaBancaria), contratos PlugBank (Payer, Account, OpenFinance), enums e interfaces (IPagadorRepository, IContaBancariaRepository, IPlugBankApi).
  - `KodiakPlugBank.Application` — casos de uso (CriarPagador, ConsultarPagadorPlugBank, ListarPagadoresPlugBank, AtualizarPagador, DesativarPagador, ObterPagadorPorCpfCnpj, AutenticarApikeyFixa, CriarConta, ListarContas, CriarExtrato, ObterExtrato) + Result/Result<T>.
  - `KodiakPlugBank.Infrastructure` — Dapper + Npgsql (repositórios, DbConnectionFactory, SchemaInitializer), cliente HTTP PlugBankApiClient, options (DatabaseOptions, PlugBankOptions), DI (AddInfrastructure).
  - `KodiakPlugBank.Api` — Minimal API: middleware de autenticação via apikey (header `X-Api-Key`), endpoints `/api/v1/payer` (POST, GET, PUT, GET /list, DELETE /{tokenPayer}), `/api/v1/account`, `/api/v1/statement/openfinance`, schema automático na inicialização.
  - `KodiakPlugBank.Tests` — xUnit (45 testes): casos de uso com fakes, PlugBankApiClient (desserialização, headers, erros) e mapeamento de variáveis de ambiente/precedência.
- Pacotes: Dapper 2.1.79, Npgsql 10.0.3, Microsoft.OpenApi 2.7.5 (correção CVE-2026-49451), Swashbuckle.AspNetCore 10.2.3.
- Documentação de uso via Swagger UI (somente em desenvolvimento): `/swagger` (UI) e `/swagger/v1/swagger.json` (JSON gerado pelo Swashbuckle). Substituiu o `MapOpenApi` nativo (`/openapi`). O middleware libera `/swagger` e `/openapi` como públicos; demais rotas exigem `X-Api-Key`.
- Swagger UI com esquema de segurança "ApiKey" configurado (`SecuritySchemeType.ApiKey`, header `X-Api-Key`) — botão "Authorize" permite testar os endpoints informando a chave. Implementação usa `OpenApiSecuritySchemeReference` (Microsoft.OpenApi 2.x, Swashbuckle 10): `options.AddSecurityRequirement(document => new OpenApiSecurityRequirement { { new OpenApiSecuritySchemeReference("ApiKey", document), [] } })`.

### Apikey fixa do KodiakERP (validação)
- Chave fixa entregue ao cliente (NUNCA armazenar em texto puro): `kdk_live_8T9hV2qLmN7xP4sRwY5ZaBcDeFgHiJkLmNoPqRsTuVwXyZ123456`.
- **No Swagger** (UI de desenvolvimento): clicar em **Authorize** e informar no campo **apiKey** (value) exatamente:
  `kdk_live_8T9hV2qLmN7xP4sRwY5ZaBcDeFgHiJkLmNoPqRsTuVwXyZ123456`
- SHA-256 real da chave armazenado na tabela `apikey_fixa` (colunas `hash_sha256` CHAR(64) UNIQUE, `descricao`, `ativo`, `criado_em`): `d7944e9b351a320a612e659fc009e8d54dfc2be0b77d0b5f1b63d2a31c5b32a3`.
  - ATENÇÃO: os hashes `6e3f4c7a...` e `8d7fbfd7...` que o usuário colou em mensagens anteriores NÃO correspondem à chave; o correto é o `d794...` calculado e validado em `docs` (vetor `sha256('abc')`).
- Tabela e seed idempotente adicionados ao `KodiakPlugBank.Infrastructure/Scripts/schema.sql` (aplicado automaticamente pelo SchemaInitializer).
- `IApikeyFixaRepository` (Core) + `ApikeyFixaRepository` Dapper (Infrastructure) com `ExisteAtivaAsync(hashSha256)`.
- `AutenticarApikeyFixaUseCase` (Application) calcula SHA-256 da chave recebida (UTF-8, lowercase hex) e valida contra o hash armazenado.
- `ApiKeyMiddleware`: valida **somente** a apikey fixa (não há mais pagador no contexto).
- **Removidas em 2026-08-01** as validações por chave mestre e por `ChaveKodiakExtrato`:
  - Chave mestre (`Security:MasterApiKey`) removida do `appsettings.json`, do middleware e da UI do Swagger.
  - `AutenticarPagadorUseCase` e `GetByChaveKodiakAsync` (interface/repo/fake) removidos.
  - `ChaveKodiakExtrato` deixou de ser obrigatória/validada no `CriarPagadorUseCase` (request agora `string?`; campo mantido no banco/entidade apenas como dado).
  - Endpoints de conta/extrato agora resolvem o pagador pelo header **`payercpfcnpj`** via `ObterPagadorPorCpfCnpjUseCase` (sem header → 401; pagador inexistente → 404).
- 4 novos testes em `Tests/UseCases/AutenticarApikeyFixaUseCaseTests.cs` (total de 42 testes).

### Rotas de Pagador (Payer) — espelham a API PlugBank (adicionado em 2026-08-02)
- `POST /api/v1/payer` — cadastra na PlugBank e no banco local (política de rate limit `bootstrap`).
- `GET /api/v1/payer` — consulta um pagador na PlugBank (header `payercpfcnpj` obrigatório; vazio → 401).
- `GET /api/v1/payer/list` — lista pagadores na PlugBank.
- `PUT /api/v1/payer` — atualiza na PlugBank e sincroniza o banco local (header `payercpfcnpj`; resolve o pagador local, 404 se não existir; 409 se o novo CPF/CNPJ já pertencer a outro pagador).
- `DELETE /api/v1/payer/{tokenPayer}` — desativa na PlugBank e marca o pagador local como inativo (coluna `ativo`); 404 se o `payercpfcnpj` não existir localmente.
- **tokenPayer**: campo `token` retornado pela PlugBank no cadastro/consulta. É armazenado na coluna `pagador.token` no cadastro (usado futuramente para conta/extrato) e é o identificador da rota DELETE.
- Contratos em `KodiakPlugBank.Core/PlugBank/Payer/` (CreatePayerRequest/Response, PayerConsultaResponse, PayerListResponse, AtualizarPayerResponse, DesativarPayerResponse).
- Endpoints locais antigos removidos: listagem local (`GET /`) e obter por id (`GET /{id}`) junto com os use cases `ListarPagadoresUseCase`/`ObterPagadorUseCase`.

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
- Autenticação exclusiva via apikey fixa (tabela `apikey_fixa`, header `X-Api-Key`). Sem chave mestre e sem validação por `ChaveKodiakExtrato` (removida em 2026-08-01).
- Endpoints espelham header/body da API PlugBank (cnpjsh, tokensh, payercpfcnpj, campos do body).
- Pagador dos endpoints de conta/extrato é identificado pelo **header `payercpfcnpj`** (via `ObterPagadorPorCpfCnpjUseCase` → `GetByCpfCnpjAsync`).
- Contas NÃO são persistidas junto ao pagador: a associação é feita posteriormente via endpoint de conta (conforme doc do projeto).
- Configuração em `appsettings.json`: `Database`, `PlugBank` (BaseUrl staging por padrão, CnpjSh, TokenSh), `ForwardedHeaders`, `RateLimiting`, `Kestrel`. Em produção, `appsettings.Production.json` substitui `PlugBank:BaseUrl` (ver seção "Ambientes de configuração").
- Nomenclatura: entidades/repositórios em pt-BR; DTOs de integração PlugBank em inglês espelhando a API.

### Como executar
- `dotnet run --project KodiakPlugBank.Api` (inicia na porta do launchSettings; schema aplicado automaticamente).
- `dotnet test KodiakPlugBank.slnx`

### Autenticação na TecnoSped PlugBank (variáveis de ambiente)
- `cnpjsh` é lido da variável de ambiente `KODIAK_PLUGBANK_SH`.
- `tokensh` é lido da variável de ambiente `KODIAK_PLUGBANK`.
- Mapeamento implementado em `KodiakPlugBank.Api/ConfigurationExtensions.cs` (precedência sobre appsettings).
- `appsettings.json` mantém apenas `PlugBank:BaseUrl` (staging por padrão).
- 6 testes cobrem o mapeamento/precedência em `Tests/Api/ConfigurationExtensionsTests.cs`.

### Ambientes de configuração (adicionado em 2026-08-02)
- `appsettings.json` — base (desenvolvimento/homologação): `PlugBank:BaseUrl` = staging, connection string local.
- `appsettings.Production.json` — contém somente `PlugBank:BaseUrl` = `https://api.pagamentobancario.com.br`. É carregado automaticamente sobre o base quando `ASPNETCORE_ENVIRONMENT=Production` (as seções informadas substituem as do base; não há merge parcial).
- **Banco de dados em produção**: NÃO colocar connection string no arquivo de produção. Definir via variável de ambiente `Database__ConnectionString` (precedência sobre os arquivos JSON, já documentado em `docs/ConfiguracaoBancoDados.md`). Sem a env var, produção usa a connection string local do `appsettings.json`.
- `CnpjSh`/`TokenSh`: continuam via variáveis de ambiente `KODIAK_PLUGBANK_SH`/`KODIAK_PLUGBANK` (mesmo em produção).
- Swagger UI habilitado **somente** em Development.
- Logs de startup exibem o `Ambiente` e o `PlugBank BaseUrl efetivo` (útil para confirmar qual host está em uso).
- Executar em produção:
  ```powershell
  $env:ASPNETCORE_ENVIRONMENT = "Production"
  dotnet run --project KodiakPlugBank.Api
  ```
  Ou publicar (`dotnet publish`) e rodar o executável com `ASPNETCORE_ENVIRONMENT=Production` definido. O default do ASP.NET Core (sem env var) já é `Production`.
- Testes de precedência da base de produção em `Tests/Api/ConfigurationExtensionsTests.cs` (3 novos, total 45 testes).

### Documentação em HTML — `KodiakPlugBank.Docs/` (adicionado em 2026-08-02)
- Site de documentação da API em **HTML5 + CSS + Bootstrap 5 + JavaScript**, independente do código .NET.
- `index.html` (sidebar lateral com módulos **Pagador**, **Conta** e **Extrato**; offcanvas em telas menores), `css/styles.css`, `js/data.js` (conteúdo = fonte de verdade), `js/app.js` (renderização por hash: `#/pagador/criar` etc.).
- **Pagador documentado** (POST/GET/GET list/PUT/DELETE); Conta e Extrato como placeholders "em breve".
- Executar: abrir `index.html` ou servir com servidor estático (ex.: `python -m http.server`).
- Contexto do projeto: `KodiakPlugBank.Docs/CONTEXTO.md`.
- **Melhorias do menu (2026-08-02):** marca "Kodiak PlugBank" no header fixo (topbar), sidebar só com menu
  (330px, sem truncamento), "Pagador" como acordeão (collapse `#sub-pagador`) e busca no menu.
- **7 melhorias de usabilidade (2026-08-02):** seletor de ambiente (Homologação/Produção) que troca a base
  `{base}` dos exemplos e o link do Swagger (persistido em localStorage); tema claro/escuro
  (`data-bs-theme` + `bi-sun`/`bi-moon`); botões "Copiar" em todos os blocos de código
  (`navigator.clipboard` com fallback); exemplos em abas **cURL / C# / PowerShell**; tabela
  "Campos da resposta" (`respostaCampos`); busca filtrando o menu (inclui submenu); botão **Swagger**
  visível somente no ambiente de homologação. Validado com `node --check` e teste jsdom (40 verificações,
  0 falhas).

### Documentos úteis
- `docs/ConfiguracaoBancoDados.md` — guia para preparar o banco de dados em **outras máquinas**
  (pré-requisitos, instalação automática via API ou manual via `psql`, connection string por variável
  de ambiente `Database__ConnectionString`, verificação e troubleshooting).
- `docs/ProjetoKodiakPlugBank.md` — visão geral e funções do projeto.
- `docs/IntegracaoPlugBankTecnoSped.md` — rotas da API PlugBank e variáveis de ambiente de autenticação.
- `docs/ConfiguracaoSeguranca.md` — proteção contra DDoS e configuração de produção (rate limiting, proxies, Kestrel).

### Pendências / próximos passos
- Configurar CnpjSh/TokenSh reais da TecnoSpeed antes de produção.
- Liberar o IP da máquina/servidor na TecnoSped para acesso à API em produção (hoje retorna 403).
- Documentar os módulos **Conta** e **Extrato** no site `KodiakPlugBank.Docs/` (em breve).
