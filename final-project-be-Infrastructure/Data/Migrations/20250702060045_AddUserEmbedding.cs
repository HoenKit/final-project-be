using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEmbeddings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmbeddingJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmbeddings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserEmbeddings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEmbeddings");
        }
    }
}
