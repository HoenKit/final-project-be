using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be_Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transaction",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "PointCost",
                table: "Transaction",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "PointChange",
                table: "Transaction",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "Transaction",
                newName: "PointCost");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Transaction",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transaction",
                newName: "PointChange");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "Transaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
