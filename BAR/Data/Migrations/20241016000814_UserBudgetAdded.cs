using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAR.Migrations
{
    /// <inheritdoc />
    public partial class UserBudgetAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBudgets",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HousingAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BillsUtilsAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GroceryDiningAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransportAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EducationAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EntertainmentAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShoppingAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MedicalAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InvestingAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MiscAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBudgets", x => x.BudgetId);
                    table.ForeignKey(
                        name: "FK_UserBudgets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBudgets_UserId",
                table: "UserBudgets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBudgets");
        }
    }
}
