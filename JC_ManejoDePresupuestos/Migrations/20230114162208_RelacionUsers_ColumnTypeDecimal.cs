using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManejoDePresupuestos.Migrations
{
    /// <inheritdoc />
    public partial class RelacionUsersColumnTypeDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Transacciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "TipoCuentas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Cuentas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Categorias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_UsuarioId",
                table: "Transacciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoCuentas_UsuarioId",
                table: "TipoCuentas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_UsuarioId",
                table: "Cuentas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UsuarioId",
                table: "Categorias",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cuentas_AspNetUsers_UsuarioId",
                table: "Cuentas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TipoCuentas_AspNetUsers_UsuarioId",
                table: "TipoCuentas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_AspNetUsers_UsuarioId",
                table: "Transacciones",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UsuarioId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Cuentas_AspNetUsers_UsuarioId",
                table: "Cuentas");

            migrationBuilder.DropForeignKey(
                name: "FK_TipoCuentas_AspNetUsers_UsuarioId",
                table: "TipoCuentas");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_AspNetUsers_UsuarioId",
                table: "Transacciones");

            migrationBuilder.DropIndex(
                name: "IX_Transacciones_UsuarioId",
                table: "Transacciones");

            migrationBuilder.DropIndex(
                name: "IX_TipoCuentas_UsuarioId",
                table: "TipoCuentas");

            migrationBuilder.DropIndex(
                name: "IX_Cuentas_UsuarioId",
                table: "Cuentas");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UsuarioId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "TipoCuentas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Cuentas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Categorias");
        }
    }
}
