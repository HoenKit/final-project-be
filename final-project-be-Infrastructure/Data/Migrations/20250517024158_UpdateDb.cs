using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_subcategories_SubCategoryId",
                table: "posts");

            migrationBuilder.DropTable(
                name: "subcategories");

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentCategoryId",
                table: "categories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                table: "categories",
                column: "ParentCategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_SubCategoryId",
                table: "posts",
                column: "SubCategoryId",
                principalTable: "categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_SubCategoryId",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_categories_ParentCategoryId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "categories");

            migrationBuilder.CreateTable(
                name: "subcategories",
                columns: table => new
                {
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subcategories", x => x.SubCategoryId);
                    table.ForeignKey(
                        name: "FK_subcategories_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subcategories_CategoryId",
                table: "subcategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_subcategories_SubCategoryId",
                table: "posts",
                column: "SubCategoryId",
                principalTable: "subcategories",
                principalColumn: "SubCategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
