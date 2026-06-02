Modelagem do banco de dados (DDL)

Arquivos:
- `schema.sql`: script DDL para criar as tabelas relacionais no PostgreSQL.

PostgreSQL / Docker
------------------
1. Inicie o PostgreSQL localmente:
```bash
docker run -d --name banco-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=banco -p 5432:5432 postgres:15
```
2. Conecte ao banco e execute o script:
```bash
docker exec -it banco-postgres psql -U postgres -d banco -f /workspace/db/schema.sql
```

> Se você não estiver usando Docker, crie o banco e depois execute `db/schema.sql` no seu cliente PostgreSQL favorito.

Criar banco manualmente:
```sql
CREATE DATABASE banco;
\c banco
```

Como aplicar o script
---------------------
- O `db/schema.sql` já está preparado para PostgreSQL.
- Ele define tipos enumerados, tabelas, chaves estrangeiras e seed de agências.
- As transações usam `UUID` nativo e as chaves Pix usam `UUID` também.

Comando CLI (psql):
```bash
psql -h localhost -p 5432 -U postgres -d banco -f db/schema.sql
```

Configuração do backend C# / EF Core
-----------------------------------
- `src/BancoApi/appsettings.json` deve apontar para:
  `Host=localhost;Port=5432;Database=banco;Username=postgres;Password=postgres`
- O projeto usa `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Garanta que a database `banco` exista antes de iniciar a API.

Observações importantes
----------------------
- `clientes.documento` permanece `VARCHAR(14)` para suportar CNPJ alfanumérico.
- `clientes.senha_hash` é `VARCHAR(255)` e deve receber hash BCrypt gerado pelo backend C#.
- `contas.saldo` é `NUMERIC(18,2) NOT NULL DEFAULT 0.00` para garantir saldo inicial zero.
- `transacoes.id` é `UUID` e deve ser gerado pelo serviço / Kafka producer.

Rodar migrations e API
----------------------
```bash
cd src/BancoApi
dotnet restore
dotnet build
# Crie uma migration PostgreSQL, se necessário
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef migrations add InitialCreate_Postgres
dotnet ef database update

dotnet run --urls http://localhost:5000
```

Dicas de teste
--------------
- Abra o Swagger em `http://localhost:5000/swagger`.
- Crie um cliente e depois use o endpoint de autenticação para obter JWT.
- Teste transferência Pix com `POST /api/pix/transferir` usando o token.

Docker Compose
--------------
Este repositório inclui um `docker-compose.yml` na raiz para subir o ambiente completo:
- PostgreSQL
- Zookeeper
- Kafka
- API .NET

Execute:
```bash
docker compose up --build
```

Depois disso, abra:
- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`

Se o PostgreSQL for iniciado pela primeira vez, carregue o esquema com:
```bash
docker exec -i banco-postgres psql -U postgres -d banco -f /app/db/schema.sql
```

> O serviço `api` usa a variável `ConnectionStrings__DefaultConnection` e `Kafka__BootstrapServers` do Docker Compose para se conectar ao Postgres e Kafka.
