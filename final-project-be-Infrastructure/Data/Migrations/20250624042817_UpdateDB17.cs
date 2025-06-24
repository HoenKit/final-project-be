using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentCourse_CouponId",
                table: "PaymentCourse");

            migrationBuilder.AlterColumn<int>(
                name: "CouponId",
                table: "PaymentCourse",
                type: "int",
                nullable: true, // cần để CouponId được optional
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCourse_CouponId",
                table: "PaymentCourse",
                column: "CouponId",
                unique: true,
                filter: "[CouponId] IS NOT NULL");
        }
    }
}
