using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdraw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountBank",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Withdraws",
                columns: table => new
                {
                    WithdrawId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MentorId = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdraws", x => x.WithdrawId);
                    table.ForeignKey(
                        name: "FK_Withdraws_Mentors_MentorId",
                        column: x => x.MentorId,
                        principalTable: "Mentors",
                        principalColumn: "MentorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Withdraws_MentorId",
                table: "Withdraws",
                column: "MentorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Withdraws");

            migrationBuilder.DropColumn(
                name: "AccountBank",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Mentors");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Mentors");
        }
    }
}
