using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be.Data.Migrations
{
	/// <inheritdoc />
	public partial class UpdateDB3 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// 1. Drop foreign key hiện tại
			migrationBuilder.DropForeignKey(
				name: "FK_Certificate_Courses_CertificateId",
				table: "Certificate");

			// 2. Drop Primary Key nếu CertificateId là PK (bạn cần kiểm tra điều này)
			migrationBuilder.DropPrimaryKey(
				name: "PK_Certificate",
				table: "Certificate");

			// 3. Drop cột CertificateId cũ
			migrationBuilder.DropColumn(
				name: "CertificateId",
				table: "Certificate");

			// 4. Thêm lại cột CertificateId có Identity
			migrationBuilder.AddColumn<int>(
				name: "CertificateId",
				table: "Certificate",
				type: "int",
				nullable: false,
				defaultValue: 0)
				.Annotation("SqlServer:Identity", "1, 1");

			// 5. Thêm lại Primary Key (nếu trước đó là PK)
			migrationBuilder.AddPrimaryKey(
				name: "PK_Certificate",
				table: "Certificate",
				column: "CertificateId");

			// 6. Thêm cột CourseId mới
			migrationBuilder.AddColumn<int>(
				name: "CourseId",
				table: "Certificate",
				type: "int",
				nullable: false,
				defaultValue: 0);

			// 7. Tạo Index trên CourseId
			migrationBuilder.CreateIndex(
				name: "IX_Certificate_CourseId",
				table: "Certificate",
				column: "CourseId",
				unique: true);

			// 8. Tạo foreign key mới
			migrationBuilder.AddForeignKey(
				name: "FK_Certificate_Courses_CourseId",
				table: "Certificate",
				column: "CourseId",
				principalTable: "Courses",
				principalColumn: "CourseId",
				onDelete: ReferentialAction.Restrict);
		}


	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Courses_CourseId",
                table: "Certificate");

            migrationBuilder.DropIndex(
                name: "IX_Certificate_CourseId",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Certificate");

            migrationBuilder.AlterColumn<int>(
                name: "CertificateId",
                table: "Certificate",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Courses_CertificateId",
                table: "Certificate",
                column: "CertificateId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
