using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class RemovePermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_Permissao_PermissionId",
                table: "Modulo");

            migrationBuilder.DropTable(
                name: "Permissao");

            migrationBuilder.DropIndex(
                name: "IX_Modulo_PermissionId",
                table: "Modulo");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "Modulo");

            migrationBuilder.AddColumn<string>(
                name: "Permissao",
                table: "Modulo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissao",
                table: "Modulo");

            migrationBuilder.AddColumn<Guid>(
                name: "PermissionId",
                table: "Modulo",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Permissao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissao", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modulo_PermissionId",
                table: "Modulo",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_Permissao_PermissionId",
                table: "Modulo",
                column: "PermissionId",
                principalTable: "Permissao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
