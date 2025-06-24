using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentCourse_Courses_CourseId",
                table: "PaymentCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentCourse_Payment_PaymentId",
                table: "PaymentCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentCourse_Courses_CourseId",
                table: "PaymentCourse",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentCourse_Payment_PaymentId",
                table: "PaymentCourse",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentCourse_Courses_CourseId",
                table: "PaymentCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentCourse_Payment_PaymentId",
                table: "PaymentCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentCourse_Courses_CourseId",
                table: "PaymentCourse",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentCourse_Payment_PaymentId",
                table: "PaymentCourse",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
