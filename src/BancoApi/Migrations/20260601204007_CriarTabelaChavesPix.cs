using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoApi.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaChavesPix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chaves_pix",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chaves_pix", x => x.id);
                    table.ForeignKey(
                        name: "FK_chaves_pix_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chaves_pix_cliente_id",
                table: "chaves_pix",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_chaves_pix_valor",
                table: "chaves_pix",
                column: "valor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chaves_pix");
        }
    }
}
