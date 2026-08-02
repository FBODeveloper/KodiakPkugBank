# CONTEXTO — Projeto de Documentação Kodiak PlugBank

## O que é
Site de documentação da **Kodiak PlugBank API** em HTML5 + CSS + Bootstrap 5 + JavaScript.
Separação do código-fonte (.NET) para que a documentação possa ser publicada de forma independente
(por exemplo, em um host estático ou via CDN).

## Estrutura do projeto (`KodiakPlugBank.Docs/`)
- `index.html` — página única com layout em duas colunas:
  - Sidebar lateral fixa (collapse via offcanvas em telas menores) com os módulos:
    **Pagador** (5 endpoints), **Conta** e **Extrato** (ainda não implementados — "em breve").
  - Área de conteúdo renderizada dinamicamente.
- `css/styles.css` — estilos customizados (tema escuro, cards, pre, tabelas, badges de método HTTP).
- `js/data.js` — `window.KODIAK_DOCS`: **fonte de verdade do conteúdo** (ambientes com URL base,
  seções, endpoints, headers, campos, exemplos em 3 linguagens com placeholder `{base}`,
  schema de resposta, erros e como usar).
- `js/app.js` — renderização + navegação por hash (`#/pagador/criar`, `#/visao-geral`, etc.),
  seletor de ambiente, tema claro/escuro, botões de copiar, abas de exemplo, busca no menu,
  marcação do item ativo e fechamento do offcanvas mobile.

## Como executar
- Basta abrir `index.html` no navegador (arquivos locais) **ou** servir a pasta com um servidor estático:
  ```powershell
  cd KodiakPlugBank.Docs
  python -m http.server 8080
  ```
  e acessar `http://localhost:8080`.
- Dependências externas via CDN: Bootstrap 5.3.3 (CSS + JS) e Bootstrap Icons 1.11.3.

## Como o conteúdo é organizado
- Cada seção de `data.js` tem `tipo`:
  - `html` — conteúdo livre (Visão Geral, Autenticação).
  - `grupo` — módulo com lista de `endpoints` (ex.: Pagador).
  - `embreve` — placeholder (Conta, Extrato).
- Cada endpoint possui: `metodo`, `rota`, `nome`, `resumo`, `oqueFaz`, `headers`,
  `campos`, `bodyExemplo`, `respostaStatus`, `respostaDescricao`, `respostaExemplo`,
  `respostaCampos` (schema da resposta), `erros`, `comoUsar` e `exemplos`
  (objeto com `curl`, `csharp` e `powershell` — todos usam o placeholder `{base}`).
- Os exemplos e a página Visão Geral trocam `{base}` pela URL base do **ambiente selecionado**
  (definido em `ambientes` no `data.js`). O botão **Swagger** usa a mesma base
  (oculto quando o ambiente não é homologação).

## Conteúdo já documentado
- **Visão Geral** — sobre a API, URL base por ambiente, índice de endpoints.
- **Autenticação** — header `X-Api-Key` (apikey fixa), header `payercpfcnpj`,
  credenciais internas (`cnpjsh`/`tokensh` via variáveis de ambiente) e rate limiting.
- **Pagador** — POST `/api/v1/payer` (criar), GET `/api/v1/payer` (consultar),
  GET `/api/v1/payer/list` (listar), PUT `/api/v1/payer` (atualizar),
  DELETE `/api/v1/payer/{tokenPayer}` (desativar).

## Funcionalidades do site
- **Seletor de ambiente** no topo (Homologação/Produção): troca a URL base de todos os exemplos
  (`{base}`) e o link do Swagger. A escolha é persistida em `localStorage` (`kodiak-docs-ambiente`).
- **Tema claro/escuro**: botão no topo alterna `data-bs-theme` no `<html>`, com ícone `bi-sun`/`bi-moon`.
  Persistido em `localStorage` (`kodiak-docs-tema`).
- **Copiar código**: todo bloco de código tem botão "Copiar" (`navigator.clipboard` com fallback).
- **Exemplos em 3 linguagens**: abas cURL / C# / PowerShell em cada endpoint.
- **Schema de resposta**: tabela "Campos da resposta" (`respostaCampos`) em cada endpoint.
- **Busca no menu**: campo centralizado no header (topbar) que filtra os itens da sidebar (inclui submenu de Pagador).
- **Botão Swagger**: link direto para `/swagger` (somente quando o ambiente é homologação).

## A fazer (próximas etapas)
- [ ] Documentar módulo **Conta** (`/api/v1/account`).
- [ ] Documentar módulo **Extrato** (`/api/v1/statement/openfinance`).

## Referências
- Código-fonte dos endpoints: `KodiakPlugBank.Api/Endpoints/PagadorEndpoints.cs`.
- Contratos: `KodiakPlugBank.Core/PlugBank/Payer/*.cs`.
- Rotas da PlugBank: `docs/IntegracaoPlugBankTecnoSped.md`.

> Observação: a apikey real NÃO aparece neste site — os exemplos usam o placeholder `SUA_APIKEY`.
