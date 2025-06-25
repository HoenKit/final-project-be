using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixReview2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rate",
                table: "Reviews",
                newName: "Rate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "Reviews",
                newName: "rate");
        }
    }
}
