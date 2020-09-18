using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AjusteOrgsUSer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organização_OrganizationId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "AspNetUsers",
                newName: "OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_OrganizationId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organização_OrganizationID",
                table: "AspNetUsers",
                column: "OrganizationID",
                principalTable: "Organização",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organização_OrganizationID",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OrganizationID",
                table: "AspNetUsers",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_OrganizationID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organização_OrganizationId",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalTable: "Organização",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
