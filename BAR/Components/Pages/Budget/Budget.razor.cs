using System.ComponentModel.DataAnnotations;
using BAR.Components.Pages.Budget.Modules;
using BAR.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace BAR.Components.Pages.Budget;

public partial class Budget
{
    // Vars
    
    // TODO: pull user's settings to get their locale (currency type)
    private ApplicationUser user = default!;
    private string userId = string.Empty;
    [CascadingParameter]
    private HttpContext context {get; set;} = default!;

    [SupplyParameterFromForm]
    private InputModel Input {get; set;} = new();
    private UserBudget bdgt;

    private string userCurrencyLocale {get; set;}
    private List<(CategoryData, List<string>)> elmts = new();
    private string? err;

    private List<string> categories = new List<string> {
        "Housing",
        "Bills/Utilities",
        "Grocery/Dining",
        "Transportation",
        "Education",
        "Debt",
        "Entertainment",
        "Shopping",
        "Medical",
        "Investing",
        "Miscellaneous"
    };

    // Functions

    // Runs when page is loaded, pseudo "constructor"-like function
    protected override async Task OnInitializedAsync()
    {
        Input.MonthlyIncome = 0.0m;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var claimsPrincipal = authState.User;
        if (claimsPrincipal.Identity.IsAuthenticated){
            user = await UserManager.GetUserAsync(claimsPrincipal);
            userId = user.Id;
        }

        // TODO: pull user data from db and populate elmts w/ CategoryData objs
        if (user is not null)
        {
            userCurrencyLocale = user.UserLocale;
            var userBudgetAsync = await dbContext.UserBudgets.Where(p => p.UserId == userId).SingleOrDefaultAsync();
            bdgt = userBudgetAsync;
            bdgt.BillsUtilsAmt = 0.0m;
            dbContext.SaveChanges();
        }
        // Initialize a generic category cell when no user data is available
        else
        {
            CategoryData defaultData = new();
            defaultData.Type = categories[0];
            defaultData.Amt = 0.0m;
            (CategoryData, List<string>) defaultElmt = (defaultData, categories);
            elmts.Add(defaultElmt);
        }

    }

    // Add an element into elmts to be rendered
    private void AddCategoryElmt()
    {
        CategoryData newData = new();
        List<string> newCats = new();
        newCats = categories.ToList();

        foreach (var elmt in elmts)
        {
            if (newCats.Contains(elmt.Item1.Type))
            {
                newCats.Remove(elmt.Item1.Type);
            }
        }
        try
        {
            if (newCats.Count < 1)
            {
                throw new Exception("You cannot add more categories.");
            }
        }
        catch (Exception e)
        {
            err = e.Message;
            return;
        }

        newData.Type = newCats[0];
        (CategoryData, List<string>) newElmt = (newData, newCats);
        elmts.Add(newElmt);
    }

    // TODO: overloaded function for element initialization on pageload, taking two params
    private void AddCategoryElmt(string type, decimal amt)
    {
        return;
    }

    // Callback function to remove element (see CategoryElmt.razor)
    private void RemoveCategoryElmt(CategoryData del)
    {
        foreach (var elmt in elmts)
        {
            try
            {
                if (elmts.Count == 1)
                {
                    throw new Exception("You must have at least one category.");
                }
            }
            catch (Exception e)
            {
                err = e.Message;
                return;
            }

            if (elmt.Item1.Equals(del))
            {
                elmts.Remove(elmt);
                break;
            }
        }
    }

    // TODO: send elmts.CategoryData and BdgtAmt to db
    private async void SaveChanges()
    {
        return;
    }

    // Clears error message out when alert is closed
    private void ClearErr()
    {
        err = string.Empty;
    }

    private sealed class InputModel {
        [Required]
        public decimal MonthlyIncome { get; set; } = 0;

        public List<CategoryData> Categories {get; set;}
    }
}