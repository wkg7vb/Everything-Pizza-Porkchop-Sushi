using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAR.Migrations
{
    /// <inheritdoc />
    public partial class UserBudgetAddDebtCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DebtAmt",
                table: "UserBudgets",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtAmt",
                table: "UserBudgets");
        }
    }
}
