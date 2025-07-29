using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Turns",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportCourse",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportCourse", x => new { x.ReportId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_ReportCourse_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportCourse_report_ReportId",
                        column: x => x.ReportId,
                        principalTable: "report",
                        principalColumn: "ReportId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportCourse_CourseId",
                table: "ReportCourse",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportCourse");

            migrationBuilder.DropColumn(
                name: "Turns",
                table: "users");
        }
    }
}
