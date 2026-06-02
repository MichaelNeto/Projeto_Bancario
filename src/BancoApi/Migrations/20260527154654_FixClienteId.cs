using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BancoApi.Migrations
{
    /// <inheritdoc />
    public partial class FixClienteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LimitesCredito_Clientes_ClienteId",
                table: "LimitesCredito");

            migrationBuilder.DropForeignKey(
                name: "FK_LimitesCredito_Clientes_ContaId",
                table: "LimitesCredito");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Clientes_ContaDestinoId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Clientes_ContaOrigemId",
                table: "Transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LimitesCredito",
                table: "LimitesCredito");

            migrationBuilder.RenameTable(
                name: "Transacoes",
                newName: "transacoes");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "clientes");

            migrationBuilder.RenameTable(
                name: "LimitesCredito",
                newName: "limites_credito");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_ContaOrigemId",
                table: "transacoes",
                newName: "IX_transacoes_ContaOrigemId");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_ContaDestinoId",
                table: "transacoes",
                newName: "IX_transacoes_ContaDestinoId");

            migrationBuilder.RenameColumn(
                name: "Uf",
                table: "clientes",
                newName: "uf");

            migrationBuilder.RenameColumn(
                name: "Telefone",
                table: "clientes",
                newName: "telefone");

            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "clientes",
                newName: "saldo");

            migrationBuilder.RenameColumn(
                name: "Logradouro",
                table: "clientes",
                newName: "logradouro");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "clientes",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Cidade",
                table: "clientes",
                newName: "cidade");

            migrationBuilder.RenameColumn(
                name: "Cep",
                table: "clientes",
                newName: "cep");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "clientes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TipoPessoa",
                table: "clientes",
                newName: "tipo_pessoa");

            migrationBuilder.RenameColumn(
                name: "TipoConta",
                table: "clientes",
                newName: "tipo_conta");

            migrationBuilder.RenameColumn(
                name: "TentativasFalhas",
                table: "clientes",
                newName: "tentativas_falhas");

            migrationBuilder.RenameColumn(
                name: "StatusCliente",
                table: "clientes",
                newName: "status_cliente");

            migrationBuilder.RenameColumn(
                name: "SenhaHash",
                table: "clientes",
                newName: "senha_hash");

            migrationBuilder.RenameColumn(
                name: "RendaMensal",
                table: "clientes",
                newName: "renda_mensal");

            migrationBuilder.RenameColumn(
                name: "NumeroEndereco",
                table: "clientes",
                newName: "numero_endereco");

            migrationBuilder.RenameColumn(
                name: "NumeroConta",
                table: "clientes",
                newName: "numero_conta");

            migrationBuilder.RenameColumn(
                name: "NumeroAgencia",
                table: "clientes",
                newName: "numero_agencia");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "clientes",
                newName: "nome_razao");

            migrationBuilder.RenameColumn(
                name: "DocumentoSecundario",
                table: "clientes",
                newName: "documento_secundario");

            migrationBuilder.RenameColumn(
                name: "DocumentoPrincipal",
                table: "clientes",
                newName: "documento");

            migrationBuilder.RenameColumn(
                name: "DataNascimentoFundacao",
                table: "clientes",
                newName: "data_nascimento_fundacao");

            migrationBuilder.RenameColumn(
                name: "DataCadastro",
                table: "clientes",
                newName: "data_cadastro");

            migrationBuilder.RenameColumn(
                name: "ChavePix",
                table: "clientes",
                newName: "chave_pix");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_NumeroConta",
                table: "clientes",
                newName: "IX_clientes_numero_conta");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_DocumentoPrincipal",
                table: "clientes",
                newName: "IX_clientes_documento");

            migrationBuilder.RenameIndex(
                name: "IX_Clientes_ChavePix",
                table: "clientes",
                newName: "IX_clientes_chave_pix");

            migrationBuilder.RenameIndex(
                name: "IX_LimitesCredito_ContaId",
                table: "limites_credito",
                newName: "IX_limites_credito_ContaId");

            migrationBuilder.RenameIndex(
                name: "IX_LimitesCredito_ClienteId",
                table: "limites_credito",
                newName: "IX_limites_credito_ClienteId");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "transacoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "transacoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "transacoes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.Sql(
                @"ALTER TABLE transacoes
                ALTER COLUMN ""DataHora""
                TYPE timestamp without time zone
                USING ""DataHora""::timestamp without time zone;"
            );

            migrationBuilder.AlterColumn<int>(
                name: "ContaOrigemId",
                table: "transacoes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ContaDestinoId",
                table: "transacoes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

                migrationBuilder.Sql(
                @"ALTER TABLE transacoes
                ALTER COLUMN ""Id""
                TYPE uuid
                USING gen_random_uuid();"
            );

            migrationBuilder.AlterColumn<string>(
                name: "uf",
                table: "clientes",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<string>(
                name: "telefone",
                table: "clientes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "logradouro",
                table: "clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "cidade",
                table: "clientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "cep",
                table: "clientes",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "clientes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "tipo_pessoa",
                table: "clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "tipo_conta",
                table: "clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "tentativas_falhas",
                table: "clientes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "status_cliente",
                table: "clientes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "senha_hash",
                table: "clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "numero_endereco",
                table: "clientes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "numero_conta",
                table: "clientes",
                type: "character varying(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "numero_agencia",
                table: "clientes",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "nome_razao",
                table: "clientes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "documento_secundario",
                table: "clientes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "documento",
                table: "clientes",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 14);

                migrationBuilder.Sql(@"
                ALTER TABLE clientes
                ALTER COLUMN data_nascimento_fundacao
                TYPE timestamp without time zone
                USING data_nascimento_fundacao::timestamp without time zone;
                ");

                migrationBuilder.Sql(@"
                ALTER TABLE clientes
                ALTER COLUMN data_cadastro
                TYPE timestamp without time zone
                USING data_cadastro::timestamp without time zone;
             ");

            migrationBuilder.AlterColumn<string>(
                name: "chave_pix",
                table: "clientes",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<int>(
                name: "StatusCredito",
                table: "limites_credito",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.Sql(@"
                ALTER TABLE limites_credito
                ALTER COLUMN ""DataConcessao""
                TYPE timestamp without time zone
                USING ""DataConcessao""::timestamp without time zone;
                ");

            migrationBuilder.AlterColumn<int>(
                name: "ContaId",
                table: "limites_credito",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "limites_credito",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "limites_credito",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_transacoes",
                table: "transacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_clientes",
                table: "clientes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_limites_credito",
                table: "limites_credito",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_limites_credito_clientes_ClienteId",
                table: "limites_credito",
                column: "ClienteId",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_limites_credito_clientes_ContaId",
                table: "limites_credito",
                column: "ContaId",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_clientes_ContaDestinoId",
                table: "transacoes",
                column: "ContaDestinoId",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_clientes_ContaOrigemId",
                table: "transacoes",
                column: "ContaOrigemId",
                principalTable: "clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_limites_credito_clientes_ClienteId",
                table: "limites_credito");

            migrationBuilder.DropForeignKey(
                name: "FK_limites_credito_clientes_ContaId",
                table: "limites_credito");

            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_clientes_ContaDestinoId",
                table: "transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_clientes_ContaOrigemId",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transacoes",
                table: "transacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_clientes",
                table: "clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_limites_credito",
                table: "limites_credito");

            migrationBuilder.RenameTable(
                name: "transacoes",
                newName: "Transacoes");

            migrationBuilder.RenameTable(
                name: "clientes",
                newName: "Clientes");

            migrationBuilder.RenameTable(
                name: "limites_credito",
                newName: "LimitesCredito");

            migrationBuilder.RenameIndex(
                name: "IX_transacoes_ContaOrigemId",
                table: "Transacoes",
                newName: "IX_Transacoes_ContaOrigemId");

            migrationBuilder.RenameIndex(
                name: "IX_transacoes_ContaDestinoId",
                table: "Transacoes",
                newName: "IX_Transacoes_ContaDestinoId");

            migrationBuilder.RenameColumn(
                name: "uf",
                table: "Clientes",
                newName: "Uf");

            migrationBuilder.RenameColumn(
                name: "telefone",
                table: "Clientes",
                newName: "Telefone");

            migrationBuilder.RenameColumn(
                name: "saldo",
                table: "Clientes",
                newName: "Saldo");

            migrationBuilder.RenameColumn(
                name: "logradouro",
                table: "Clientes",
                newName: "Logradouro");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Clientes",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "cidade",
                table: "Clientes",
                newName: "Cidade");

            migrationBuilder.RenameColumn(
                name: "cep",
                table: "Clientes",
                newName: "Cep");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Clientes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tipo_pessoa",
                table: "Clientes",
                newName: "TipoPessoa");

            migrationBuilder.RenameColumn(
                name: "tipo_conta",
                table: "Clientes",
                newName: "TipoConta");

            migrationBuilder.RenameColumn(
                name: "tentativas_falhas",
                table: "Clientes",
                newName: "TentativasFalhas");

            migrationBuilder.RenameColumn(
                name: "status_cliente",
                table: "Clientes",
                newName: "StatusCliente");

            migrationBuilder.RenameColumn(
                name: "senha_hash",
                table: "Clientes",
                newName: "SenhaHash");

            migrationBuilder.RenameColumn(
                name: "renda_mensal",
                table: "Clientes",
                newName: "RendaMensal");

            migrationBuilder.RenameColumn(
                name: "numero_endereco",
                table: "Clientes",
                newName: "NumeroEndereco");

            migrationBuilder.RenameColumn(
                name: "numero_conta",
                table: "Clientes",
                newName: "NumeroConta");

            migrationBuilder.RenameColumn(
                name: "numero_agencia",
                table: "Clientes",
                newName: "NumeroAgencia");

            migrationBuilder.RenameColumn(
                name: "nome_razao",
                table: "Clientes",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "documento_secundario",
                table: "Clientes",
                newName: "DocumentoSecundario");

            migrationBuilder.RenameColumn(
                name: "documento",
                table: "Clientes",
                newName: "DocumentoPrincipal");

            migrationBuilder.RenameColumn(
                name: "data_nascimento_fundacao",
                table: "Clientes",
                newName: "DataNascimentoFundacao");

            migrationBuilder.RenameColumn(
                name: "data_cadastro",
                table: "Clientes",
                newName: "DataCadastro");

            migrationBuilder.RenameColumn(
                name: "chave_pix",
                table: "Clientes",
                newName: "ChavePix");

            migrationBuilder.RenameIndex(
                name: "IX_clientes_numero_conta",
                table: "Clientes",
                newName: "IX_Clientes_NumeroConta");

            migrationBuilder.RenameIndex(
                name: "IX_clientes_documento",
                table: "Clientes",
                newName: "IX_Clientes_DocumentoPrincipal");

            migrationBuilder.RenameIndex(
                name: "IX_clientes_chave_pix",
                table: "Clientes",
                newName: "IX_Clientes_ChavePix");

            migrationBuilder.RenameIndex(
                name: "IX_limites_credito_ContaId",
                table: "LimitesCredito",
                newName: "IX_LimitesCredito_ContaId");

            migrationBuilder.RenameIndex(
                name: "IX_limites_credito_ClienteId",
                table: "LimitesCredito",
                newName: "IX_LimitesCredito_ClienteId");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Transacoes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Transacoes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "Transacoes",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

                migrationBuilder.Sql(
                @"ALTER TABLE transacoes
                ALTER COLUMN ""DataHora""
                TYPE TEXT
                USING ""DataHora""::TEXT;"
               );

            migrationBuilder.AlterColumn<int>(
                name: "ContaOrigemId",
                table: "Transacoes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ContaDestinoId",
                table: "Transacoes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

                migrationBuilder.Sql(
                @"ALTER TABLE transacoes
                ALTER COLUMN ""Id""
                TYPE TEXT
                USING ""Id""::TEXT;"
              );

            migrationBuilder.AlterColumn<string>(
                name: "Uf",
                table: "Clientes",
                type: "TEXT",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Logradouro",
                table: "Clientes",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Cidade",
                table: "Clientes",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Cep",
                table: "Clientes",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "TipoPessoa",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "TipoConta",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "TentativasFalhas",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "StatusCliente",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SenhaHash",
                table: "Clientes",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroEndereco",
                table: "Clientes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroConta",
                table: "Clientes",
                type: "TEXT",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroAgencia",
                table: "Clientes",
                type: "TEXT",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentoSecundario",
                table: "Clientes",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentoPrincipal",
                table: "Clientes",
                type: "TEXT",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14);

            migrationBuilder.AlterColumn<string>(
                name: "DataNascimentoFundacao",
                table: "Clientes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "DataCadastro",
                table: "Clientes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ChavePix",
                table: "Clientes",
                type: "TEXT",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<int>(
                name: "StatusCredito",
                table: "LimitesCredito",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.Sql(@"
                ALTER TABLE ""LimitesCredito""
                ALTER COLUMN ""DataConcessao""
                TYPE TEXT
                USING ""DataConcessao""::TEXT;
                ");

            migrationBuilder.AlterColumn<int>(
                name: "ContaId",
                table: "LimitesCredito",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "LimitesCredito",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LimitesCredito",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transacoes",
                table: "Transacoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LimitesCredito",
                table: "LimitesCredito",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LimitesCredito_Clientes_ClienteId",
                table: "LimitesCredito",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LimitesCredito_Clientes_ContaId",
                table: "LimitesCredito",
                column: "ContaId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Clientes_ContaDestinoId",
                table: "Transacoes",
                column: "ContaDestinoId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Clientes_ContaOrigemId",
                table: "Transacoes",
                column: "ContaOrigemId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
