using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspireHubWebApp.Migrations
{
    public partial class DatabaseFixing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CourseDetail");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TrainingCourseDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TrainingCourseDetails");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CourseDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
