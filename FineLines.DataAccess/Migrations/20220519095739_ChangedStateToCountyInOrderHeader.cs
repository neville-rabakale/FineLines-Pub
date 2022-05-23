using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FineLinesApp.Migrations
{
    public partial class ChangedStateToCountyInOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "OrderHeaders",
                newName: "County");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "County",
                table: "OrderHeaders",
                newName: "State");
        }
    }
}
