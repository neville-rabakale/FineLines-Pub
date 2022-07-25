using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FineLinesApp.Migrations
{
    public partial class UpdatedProductToIncludePackaging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "CoverTypeId",
                table: "Products",
                newName: "PackagingId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products",
                newName: "IX_Products_PackagingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Packagings_PackagingId",
                table: "Products",
                column: "PackagingId",
                principalTable: "Packagings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Packagings_PackagingId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "PackagingId",
                table: "Products",
                newName: "CoverTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_PackagingId",
                table: "Products",
                newName: "IX_Products_CoverTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products",
                column: "CoverTypeId",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
