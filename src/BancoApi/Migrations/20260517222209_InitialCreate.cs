using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoPessoa = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DocumentoPrincipal = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    DocumentoSecundario = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DataNascimentoFundacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NumeroEndereco = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Uf = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    RendaMensal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumeroAgencia = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    NumeroConta = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    TipoConta = table.Column<int>(type: "INTEGER", nullable: false),
                    ChavePix = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    StatusCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TentativasFalhas = table.Column<int>(type: "INTEGER", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LimitesCredito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContaId = table.Column<int>(type: "INTEGER", nullable: false),
                    RendaDeclarada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LimiteLiberado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusCredito = table.Column<int>(type: "INTEGER", nullable: false),
                    DataConcessao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitesCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LimitesCredito_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LimitesCredito_Clientes_ContaId",
                        column: x => x.ContaId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContaOrigemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContaDestinoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Clientes_ContaDestinoId",
                        column: x => x.ContaDestinoId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacoes_Clientes_ContaOrigemId",
                        column: x => x.ContaOrigemId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ChavePix",
                table: "Clientes",
                column: "ChavePix",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DocumentoPrincipal",
                table: "Clientes",
                column: "DocumentoPrincipal",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_NumeroConta",
                table: "Clientes",
                column: "NumeroConta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LimitesCredito_ClienteId",
                table: "LimitesCredito",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_LimitesCredito_ContaId",
                table: "LimitesCredito",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaDestinoId",
                table: "Transacoes",
                column: "ContaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaOrigemId",
                table: "Transacoes",
                column: "ContaOrigemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LimitesCredito");

            migrationBuilder.DropTable(
                name: "Transacoes");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
