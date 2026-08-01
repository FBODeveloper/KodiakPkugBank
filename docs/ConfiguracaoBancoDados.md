# Configuração do Banco de Dados — Kodiak PlugBank

Documento para preparação do ambiente de banco de dados em **outras máquinas/programadores**.

## Pré-requisitos

- **PostgreSQL 18** instalado e rodando (porta padrão `5432`).
- Usuário com permissão de **criar banco** e **criar tabelas** (o padrão do projeto é `postgres`).
- SDK **.NET 10** (apenas se for usar a instalação automática via API).

> Nota: o projeto foi desenvolvido para Postgres 18, mas o schema usa apenas recursos básicos
> e deve funcionar em versões superiores ou recentes (14+).

## Como o banco é criado

Existem duas formas de criar o banco:

### Opção A — Automática (recomendada)

A API aplica o schema sozinha na inicialização:

1. Configure a conexão (veja abaixo).
2. Inicie a API:
   ```powershell
   dotnet run --project KodiakPlugBank.Api
   ```
3. O `SchemaInitializer` (`KodiakPlugBank.Infrastructure/Data/SchemaInitializer.cs`):
   - Cria o database `kodiak_plugbank` **se não existir** (conectando no banco de manutenção `postgres`);
   - Executa o script `KodiakPlugBank.Infrastructure/Scripts/schema.sql` (embutido no assembly como EmbeddedResource);
   - O log exibe: `Schema do banco de dados verificado/aplicado com sucesso.`

### Opção B — Manual (via `psql`)

Quando não quiser depender da inicialização automática:

1. Criar o banco:
   ```powershell
   & "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -p 5432 -U postgres -c "CREATE DATABASE kodiak_plugbank;"
   ```
   (o comando pedirá a senha do usuário `postgres`)

2. Executar o script:
   ```powershell
   & "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -p 5432 -U postgres -d kodiak_plugbank -f "KodiakPlugBank.Infrastructure\Scripts\schema.sql"
   ```
   > O caminho do `psql.exe` pode variar conforme a instalação.

## Configuração da conexão

### 1. `appsettings.json` (padrão de desenvolvimento)

```json
"Database": {
  "ConnectionString": "Host=localhost;Port=5432;Database=kodiak_plugbank;Username=postgres;Password=123!asd"
}
```

### 2. Variável de ambiente (recomendado para outras máquinas)

Sobrescreve o `appsettings.json` sem alterar código:

```powershell
$env:Database__ConnectionString = "Host=localhost;Port=5432;Database=kodiak_plugbank;Username=SEU_USUARIO;Password=SUA_SENHA"
dotnet run --project KodiakPlugBank.Api
```

> A notação `__` (dois underscores) é convertida pelo .NET para `:` e permite sobrescrever
> qualquer seção do `appsettings.json` via variável de ambiente.

### Parâmetros úteis da connection string (Npgsql)

| Parâmetro  | Exemplo                          | Descrição                            |
|------------|----------------------------------|--------------------------------------|
| Host       | `localhost` ou IP da máquina     | Endereço do servidor Postgres        |
| Port       | `5432`                           | Porta do servidor                    |
| Database   | `kodiak_plugbank`                | Nome do banco                        |
| Username   | `postgres`                       | Usuário com permissão de escrita     |
| Password   | `123!asd`                        | Senha do usuário                     |

## Estrutura criada pelo script

O script `schema.sql` cria (usando `IF NOT EXISTS`, então é idempotente):

- **`pagador`**
  - `id` (serial, PK), `nome`, `email`, `cpf_cnpj` (único), endereço (`logradouro`, `bairro`,
    `numero_endereco`, `complemento_endereco`, `cidade`, `estado`, `cep`),
    `token`, `statement_ativado`, `chave_kodiak_extrato` (varchar 1000, opcional, não é usado para validação), `criado_em`.
- **`conta_bancaria`**
  - `id` (serial, PK), `id_pagador` (FK), `id_conta_bancaria_kodiak` (único),
    `account_hash` (índice único parcial), dados bancários (`bank_code`, `agency`,
    `account_number`, dígitos, convênios, remessa), flags (`statement_ativado`,
    `account_payment`, `governmental_resource`, `dda_ativado`, etc.), `openfinance_link`, `criado_em`.

## Verificação

Listar as tabelas:

```powershell
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -h localhost -p 5432 -U postgres -d kodiak_plugbank -c "\dt"
```

Saída esperada:

```
 public  | conta_bancaria | tabela | postgres
 public  | pagador        | tabela | postgres
```

## Troubleshooting

| Problema                                            | Solução                                                        |
|-----------------------------------------------------|----------------------------------------------------------------|
| `Falha ao verificar/aplicar o schema do banco de dados` no log | Confirme que o Postgres está rodando e que a connection string está correta. |
| `database "kodiak_plugbank" does not exist` (acesso manual) | Execute a Opção B passo 1 (criar banco) ou inicie a API ao menos uma vez. |
| Usuário sem permissão para criar banco              | Ajuste a `Username` para um usuário com privilégios, ou crie o banco manualmente. |
| Porta diferente de `5432`                           | Ajuste `Port` na connection string.                              |
