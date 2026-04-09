using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDocsToJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverLetter",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResumeLink",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverLetter",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "ResumeLink",
                table: "JobApplications");
        }
    }
}
