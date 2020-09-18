using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class OrgsModules5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_Organização_OrganizationId",
                table: "Modulo");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_Organização_OrganizationId",
                table: "Modulo",
                column: "OrganizationId",
                principalTable: "Organização",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_Organização_OrganizationId",
                table: "Modulo");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_Organização_OrganizationId",
                table: "Modulo",
                column: "OrganizationId",
                principalTable: "Organização",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
