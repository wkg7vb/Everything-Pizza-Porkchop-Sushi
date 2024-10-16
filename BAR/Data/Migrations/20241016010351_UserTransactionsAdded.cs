using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAR.Migrations
{
    /// <inheritdoc />
    public partial class UserTransactionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAvatarColor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "#E5A000");

            migrationBuilder.AddColumn<bool>(
                name: "UserDarkmode",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserFName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserLName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserLocale",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "en-US");

            migrationBuilder.CreateTable(
                name: "UserTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransasctionCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_UserTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactions_UserId",
                table: "UserTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTransactions");

            migrationBuilder.DropColumn(
                name: "UserAvatarColor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserDarkmode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserFName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserLName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserLocale",
                table: "AspNetUsers");
        }
    }
}
