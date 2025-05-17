using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_SubCategoryId",
                table: "posts");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "posts",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_SubCategoryId",
                table: "posts",
                newName: "IX_posts_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_CategoryId",
                table: "posts",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_CategoryId",
                table: "posts");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "posts",
                newName: "SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_CategoryId",
                table: "posts",
                newName: "IX_posts_SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_SubCategoryId",
                table: "posts",
                column: "SubCategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
