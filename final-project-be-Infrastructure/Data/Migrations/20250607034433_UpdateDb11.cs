using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackUrl",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "FontUrl",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "MentorCertificateId",
                table: "Mentors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackUrl",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontUrl",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MentorCertificateId",
                table: "Mentors",
                type: "int",
                nullable: true);
        }
    }
}
