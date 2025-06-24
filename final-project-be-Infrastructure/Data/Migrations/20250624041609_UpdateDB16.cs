using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa index cũ (có thể là unique)
            migrationBuilder.DropIndex(
                name: "IX_PaymentCourse_CouponId",
                table: "PaymentCourse");

            // Đảm bảo CouponId là nullable (nếu cần)
            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "PaymentCourse",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Tạo lại index KHÔNG UNIQUE
            migrationBuilder.CreateIndex(
                name: "IX_PaymentCourse_CouponId",
                table: "PaymentCourse",
                column: "CouponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentCourse_CouponId",
                table: "PaymentCourse");

            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "PaymentCourse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCourse_CouponId",
                table: "PaymentCourse",
                column: "CouponId",
                unique: true);
        }
    }
}
