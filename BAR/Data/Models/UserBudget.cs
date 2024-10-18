using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BAR.Data.Models;

public class UserBudget
{
    [Key]
    public int BudgetId { get; set; }
    public decimal MonthlyIncome { get; set; } = 0.0m;
    public decimal? HousingAmt { get; set; }
    public decimal? BillsUtilsAmt { get; set; }
    public decimal? GroceryDiningAmt { get; set; }
    public decimal? TransportAmt { get; set; }
    public decimal? EducationAmt { get; set; }
    public decimal? DebtAmt {get; set;}
    public decimal? EntertainmentAmt { get; set; }
    public decimal? ShoppingAmt { get; set; }
    public decimal? MedicalAmt { get; set; }
    public decimal? InvestingAmt { get; set; }
    public decimal? MiscAmt { get; set; }

    // FK from ApplicationUser
    public string UserId {get; set;}
    public ApplicationUser User { get; set; }

    [NotMapped]
    public bool AllCategoriesNull {
        get {
            return 
            HousingAmt == null
            && BillsUtilsAmt == null
            && GroceryDiningAmt == null
            && TransportAmt == null
            && EducationAmt == null
            && DebtAmt == null
            && EntertainmentAmt == null
            && ShoppingAmt == null
            && MedicalAmt == null
            && InvestingAmt == null
            && MiscAmt == null;
        }
    }
}