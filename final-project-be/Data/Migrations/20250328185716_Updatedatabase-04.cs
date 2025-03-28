using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updatedatabase04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserMetadataId",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserMetadataId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
