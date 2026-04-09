using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExpandedCoursesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Courses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Courses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Courses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Courses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Courses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Syllabus",
                table: "Courses",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "CourseID",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Description", "Duration", "InstructorId", "IsActive", "Level", "Price", "Syllabus" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Learn to build robust REST APIs with ASP.NET Core, Entity Framework, and SQL Server.", "8 Weeks", null, true, "Intermediate", 0m, null });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_InstructorId",
                table: "Courses",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_InstructorId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Syllabus",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
