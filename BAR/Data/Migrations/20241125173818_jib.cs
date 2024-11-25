using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAR.Migrations
{
    /// <inheritdoc />
    public partial class jib : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransasctionCategory",
                table: "UserTransactions",
                newName: "TransactionCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionCategory",
                table: "UserTransactions",
                newName: "TransasctionCategory");
        }
    }
}
