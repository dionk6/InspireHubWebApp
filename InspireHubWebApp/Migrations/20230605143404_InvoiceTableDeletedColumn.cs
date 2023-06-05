using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireHubWebApp.Migrations
{
    public partial class InvoiceTableDeletedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Invoices");
        }
    }
}
