# Segurança da API (proteção contra DDoS)

Defesas implementadas em camada de aplicação (todas nativas do ASP.NET Core, sem pacotes de terceiros).

## 1. Rate limiting (limitação de requisições por IP)

- Implementado com `Microsoft.AspNetCore.RateLimiting` (janela deslizante, partição por IP real do cliente).
- Aplica-se ANTES do middleware de autenticação: requisições acima do limite são recusadas sem processamento.
- Resposta padrão: `429 Too Many Requests` com header `Retry-After` e corpo JSON.

### Políticas (configuráveis em `appsettings.json` → `RateLimiting`)

| Política | Onde se aplica | Padrão | Efeito |
|---|---|---|---|
| `Global` | Todas as rotas | 100 req / 10s (janela deslizante, sem fila) | Proteção geral por IP |
| `Bootstrap` | `POST /api/v1/payer` | 10 req / 60s | Cadastro de pagador (bootstrap) mais restrito |

Ajuste conforme o volume esperado do KodiakERP. Valores muito baixos podem gerar 429 legítimos;
valores muito altos reduzem a proteção. `QueueLimit > 0` faz requisições excedentes esperarem em fila
em vez de receberem 429 imediato.

## 2. Identificação do cliente atrás de proxy/CDN (Forwarded Headers)

- A API é exposta atrás de Cloudflare/nginx (decisão de arquitetura).
- `UseForwardedHeaders()` traduz `X-Forwarded-For`/`X-Forwarded-Proto`, fazendo o rate limiter enxergar o
  **IP real** do cliente (não o IP do proxy) e o esquema HTTPS ser preservado.
- Configuração em `appsettings.json` → `ForwardedHeaders`:
  - `ForwardLimit`: quantidade máxima de hops confiáveis (1 por padrão).
  - `KnownProxies`: IPs dos proxies intermediários autorizados a enviar o header.
  - `KnownNetworks`: faixas de rede dos proxies, formato `IP/prefixo` (ex.: `173.245.48.0/20`).

> **IMPORTANTE:** em produção, informe as faixas de IP do seu proxy/CDN em `KnownNetworks`
> (para Cloudflare, use a lista oficial em https://www.cloudflare.com/ips/). Sem isso o header pode
> ser falsificado e o atacante "mudar de IP" a cada requisição, driblando o rate limit.

## 3. Limites do servidor Kestrel (`appsettings.json` → `Kestrel:Limits`)

| Configuração | Padrão | Protege contra |
|---|---|---|
| `MaxRequestBodySize` | 10 MB (10.485.760 bytes) | Payloads gigantes (exaustão de memória) |
| `MaxConcurrentConnections` | 500 | Exaustão de conexões |
| `MaxConcurrentUpgradedConnections` | 100 | Abuso de WebSockets |
| `KeepAliveTimeout` | 30s | Conexões ociosas mantidas abertas |
| `RequestHeadersTimeout` | 5s | Headers maliciosos enviados lentamente |
| `MinRequestBodyDataRate` | 240 B/s, 5s de tolerância | **Slowloris** (corpo enviado devagar) |
| `MinResponseDataRate` | 240 B/s, 5s de tolerância | **Slowloris** (leitura lenta da resposta) |

## 4. Headers de segurança nas respostas

Middleware `SecurityHeadersMiddleware` adiciona em toda resposta:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Referrer-Policy: no-referrer`

## 5. Ordem do pipeline (importante)

```
UseForwardedHeaders → SecurityHeaders → Swagger (dev) → RateLimiter → ApiKeyMiddleware → Endpoints
```

O rate limiter roda antes da autenticação para não gastar CPU/BD em requisições que serão recusadas.

## 6. Recomendações para produção (fora desta aplicação)

- CDN/WAF (ex.: Cloudflare) para absorver DDoS volumétrico (camadas de rede/transporte) — esta API protege
  a camada de aplicação; o volumétrico é papel do CDN.
- HTTPS obrigatório (terminação no proxy/CDN).
- Monitorar logs de `429` (logger "RateLimiter") para detectar ataques e calibrar os limites.
- A autenticação é validada exclusivamente pela apikey fixa (tabela `apikey_fixa`, hash SHA-256) no header `X-Api-Key`.
