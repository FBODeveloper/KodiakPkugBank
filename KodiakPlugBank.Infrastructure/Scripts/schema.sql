CREATE TABLE IF NOT EXISTS pagador (
    id                      SERIAL PRIMARY KEY,
    nome                    VARCHAR(250)     NOT NULL,
    email                   VARCHAR(250),
    cpf_cnpj                VARCHAR(18)      NOT NULL UNIQUE,
    logradouro              VARCHAR(250),
    bairro                  VARCHAR(250),
    numero_endereco         VARCHAR(10),
    complemento_endereco    VARCHAR(250),
    cidade                  VARCHAR(250),
    estado                  VARCHAR(2),
    cep                     VARCHAR(10),
    token                   VARCHAR(250),
    statement_ativado       BOOLEAN          NOT NULL DEFAULT FALSE,
    chave_kodiak_extrato    VARCHAR(1000)    NOT NULL,
    criado_em               TIMESTAMP        NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS conta_bancaria (
    id                         SERIAL PRIMARY KEY,
    id_pagador                 INT           NOT NULL REFERENCES pagador(id),
    id_conta_bancaria_kodiak   INT           NOT NULL,
    account_hash               VARCHAR(255),
    bank_code                  VARCHAR(3),
    agency                     VARCHAR(10),
    agency_digit               VARCHAR(2),
    account_number             VARCHAR(12),
    account_number_digit       VARCHAR(2),
    account_dac                VARCHAR(2),
    account_type               VARCHAR(10),
    account_payment            BOOLEAN       NOT NULL DEFAULT FALSE,
    governmental_resource      BOOLEAN       NOT NULL DEFAULT FALSE,
    convenio_agency            VARCHAR(20),
    convenio_number            VARCHAR(20),
    remessa_sequential         BIGINT,
    webservice                 BOOLEAN       NOT NULL DEFAULT FALSE,
    code_contract              VARCHAR(100),
    dda_ativado                BOOLEAN       NOT NULL DEFAULT FALSE,
    client_key                 VARCHAR(255),
    client_secret              VARCHAR(255),
    client_id                  VARCHAR(255),
    recipient_notification     BOOLEAN       NOT NULL DEFAULT FALSE,
    statement_ativado          BOOLEAN       NOT NULL DEFAULT FALSE,
    pagbb_ativado              BOOLEAN       NOT NULL DEFAULT FALSE,
    openfinance_link           TEXT,
    criado_em                  TIMESTAMP     NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_conta_bancaria_kodiak UNIQUE (id_conta_bancaria_kodiak),
    CONSTRAINT uq_pagador_conta_kodiak UNIQUE (id_pagador, id_conta_bancaria_kodiak)
);

CREATE INDEX IF NOT EXISTS ix_conta_bancaria_id_pagador ON conta_bancaria (id_pagador);

CREATE UNIQUE INDEX IF NOT EXISTS ux_conta_bancaria_account_hash
    ON conta_bancaria (account_hash)
    WHERE account_hash IS NOT NULL;

CREATE TABLE IF NOT EXISTS apikey_fixa (
    id            SERIAL PRIMARY KEY,
    hash_sha256   CHAR(64)     NOT NULL UNIQUE,
    descricao     VARCHAR(250),
    ativo         BOOLEAN      NOT NULL DEFAULT TRUE,
    criado_em     TIMESTAMP    NOT NULL DEFAULT NOW()
);

INSERT INTO apikey_fixa (hash_sha256, descricao)
SELECT 'd7944e9b351a320a612e659fc009e8d54dfc2be0b77d0b5f1b63d2a31c5b32a3', 'apikey fixa KodiakERP (kdk_live)'
WHERE NOT EXISTS (
    SELECT 1 FROM apikey_fixa
    WHERE hash_sha256 = 'd7944e9b351a320a612e659fc009e8d54dfc2be0b77d0b5f1b63d2a31c5b32a3'
);
