DO $$ BEGIN CREATE TYPE tipo_pessoa_enum AS ENUM ('PF', 'PJ'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;
DO $$ BEGIN CREATE TYPE status_cliente_enum AS ENUM ('ATIVO', 'BLOQUEADO', 'INATIVO'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;
DO $$ BEGIN CREATE TYPE tipo_conta_enum AS ENUM ('CORRENTE', 'POUPANCA'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;
DO $$ BEGIN CREATE TYPE status_credito_enum AS ENUM ('APROVADO', 'NEGADO'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;
DO $$ BEGIN CREATE TYPE tipo_transacao_enum AS ENUM ('PIX_ENVIADO', 'PIX_RECEBIDO'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;
DO $$ BEGIN CREATE TYPE status_transacao_enum AS ENUM ('PROCESSANDO', 'CONCLUIDO', 'FALHOU'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;

CREATE TABLE IF NOT EXISTS agencias (
  id        INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  numero    CHAR(4)   NOT NULL UNIQUE,
  nome      VARCHAR(100),
  criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS clientes (
  id                       INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  tipo_pessoa              tipo_pessoa_enum    NOT NULL,
  nome_razao               VARCHAR(200)        NOT NULL,
  documento                VARCHAR(14)         NOT NULL,
  documento_secundario     VARCHAR(50),
  data_nascimento_fundacao DATE,
  email                    VARCHAR(200)        NOT NULL,
  telefone                 VARCHAR(20)         NOT NULL,
  logradouro               VARCHAR(200)        NOT NULL,
  numero_endereco          VARCHAR(20)         NOT NULL,
  cep                      VARCHAR(10)         NOT NULL,
  cidade                   VARCHAR(100)        NOT NULL,
  uf                       CHAR(2)             NOT NULL,
  renda_mensal             NUMERIC(18,2)       NOT NULL,
  senha_hash               VARCHAR(255)        NOT NULL,
  tentativas_falhas        INT                 NOT NULL DEFAULT 0,
  status_cliente           status_cliente_enum NOT NULL DEFAULT 'ATIVO',
  data_cadastro            TIMESTAMP           NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT uq_clientes_documento UNIQUE (documento)
);

CREATE TABLE IF NOT EXISTS contas (
  id           INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  cliente_id   INT             NOT NULL,
  agencia_id   INT             NOT NULL,
  numero_conta VARCHAR(12)     NOT NULL UNIQUE,
  tipo_conta   tipo_conta_enum NOT NULL,
  saldo        NUMERIC(18,2)   NOT NULL DEFAULT 0.00,
  chave_pix    UUID            NOT NULL UNIQUE,
  criado_em    TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_contas_cliente FOREIGN KEY (cliente_id) REFERENCES clientes(id) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_contas_agencia FOREIGN KEY (agencia_id) REFERENCES agencias(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS limites_credito (
  id              INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  cliente_id      INT                 NOT NULL,
  renda_declarada NUMERIC(18,2)       NOT NULL,
  limite_liberado NUMERIC(18,2)       NOT NULL,
  status_credito  status_credito_enum NOT NULL,
  data_concessao  TIMESTAMP           NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_limites_cliente FOREIGN KEY (cliente_id) REFERENCES clientes(id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS transacoes (
  id               UUID                  NOT NULL PRIMARY KEY,
  data_hora        TIMESTAMP             NOT NULL DEFAULT CURRENT_TIMESTAMP,
  tipo             tipo_transacao_enum   NOT NULL,
  status           status_transacao_enum NOT NULL,
  valor            NUMERIC(18,2)         NOT NULL,
  conta_origem_id  INT                   NOT NULL,
  conta_destino_id INT,
  observacao       VARCHAR(200),
  CONSTRAINT fk_transacoes_origem  FOREIGN KEY (conta_origem_id)  REFERENCES contas(id) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_transacoes_destino FOREIGN KEY (conta_destino_id) REFERENCES contas(id) ON DELETE RESTRICT ON UPDATE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_clientes_email  ON clientes(email);
CREATE INDEX IF NOT EXISTS idx_contas_chavepix ON contas(chave_pix);

INSERT INTO agencias (numero, nome) VALUES
  ('0001', 'Agência Central'),
  ('0002', 'Agência Norte'),
  ('0003', 'Agência Sul')
ON CONFLICT (numero) DO NOTHING;
