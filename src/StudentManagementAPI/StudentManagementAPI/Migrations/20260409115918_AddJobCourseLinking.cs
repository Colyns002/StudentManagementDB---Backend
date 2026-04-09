using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddJobCourseLinking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobPostCourses",
                columns: table => new
                {
                    JobPostId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    RecommendationLevel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostCourses", x => new { x.JobPostId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_JobPostCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobPostCourses_JobPosts_JobPostId",
                        column: x => x.JobPostId,
                        principalTable: "JobPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPostCourses_CourseId",
                table: "JobPostCourses",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobPostCourses");
        }
    }
}
