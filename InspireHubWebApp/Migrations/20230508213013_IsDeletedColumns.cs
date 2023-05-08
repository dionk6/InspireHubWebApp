using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireHubWebApp.Migrations
{
    public partial class IsDeletedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Training",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseDetail");
        }
    }
}
