using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updatedatabase03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostId",
                table: "subcategories");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "users",
                newName: "UserMetadataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserMetadataId",
                table: "users",
                newName: "RoleId");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "subcategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
