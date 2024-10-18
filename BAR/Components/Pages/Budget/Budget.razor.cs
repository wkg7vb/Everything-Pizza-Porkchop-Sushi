using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using BAR.Components.Pages.Budget.Modules;
using BAR.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BAR.Components.Pages.Budget;

public partial class Budget
{
    // Vars
    // User vars from db (init'd inside OnInitialiedAsync)
    private ApplicationUser user = default!;
    private UserBudget bdgt;

    // Local vars
    private string userCurrencyLocale {get; set;}
    private List<CategoryData> elmts = new();
    private string? err;
    private string? msg;
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
    // Runs twice on interactive pages
    // TODO: look at how to prevent the render code from running twice
    protected override async Task OnInitializedAsync()
    {
        // Attain authenticaiton state and user
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var claimsPrincipal = authState.User;
        if (claimsPrincipal.Identity.IsAuthenticated){
            user = await UserManager.GetUserAsync(claimsPrincipal);
        }

        // Init local vars from db
        if (user is not null)
        {
            userCurrencyLocale = user.UserLocale;
            bdgt = await dbContext.UserBudgets.SingleOrDefaultAsync(p => p.UserId == user.Id);
        }
        else throw new Exception("Invalid user.");
        
        // Populate elmts list from db
        if (bdgt is not null)
        {
            if (bdgt.HousingAmt is not null) AddCategoryElmt(type: "Housing", amt: bdgt.HousingAmt);
            if (bdgt.BillsUtilsAmt is not null) AddCategoryElmt(type: "Bills/Utilities", amt: bdgt.BillsUtilsAmt);
            if (bdgt.GroceryDiningAmt is not null) AddCategoryElmt(type: "Grocery/Dining", amt: bdgt.GroceryDiningAmt);
            if (bdgt.TransportAmt is not null) AddCategoryElmt(type: "Transportation", amt: bdgt.TransportAmt);            
            if (bdgt.EducationAmt is not null) AddCategoryElmt(type: "Education", amt: bdgt.EducationAmt);
            if (bdgt.DebtAmt is not null) AddCategoryElmt(type: "Debt", amt: bdgt.DebtAmt);
            if (bdgt.EntertainmentAmt is not null) AddCategoryElmt(type: "Entertainment", amt: bdgt.EntertainmentAmt);
            if (bdgt.ShoppingAmt is not null) AddCategoryElmt(type: "Shopping", amt: bdgt.ShoppingAmt);
            if (bdgt.MedicalAmt is not null) AddCategoryElmt(type: "Medical", amt: bdgt.MedicalAmt);
            if (bdgt.InvestingAmt is not null) AddCategoryElmt(type: "Investing", amt: bdgt.InvestingAmt);
            if (bdgt.MiscAmt is not null) AddCategoryElmt(type: "Miscellaneous", amt: bdgt.MiscAmt);
            if (elmts.Count() == 0 && bdgt.AllCategoriesNull) AddCategoryElmt();
        }
        // Initialize a generic category cell when no user data is available
        else
        {
            bdgt = new UserBudget{
                UserId = user.Id,
                User = user
            };
            await dbContext.UserBudgets.AddAsync(bdgt);
            await dbContext.SaveChangesAsync();
        }
    }

    // Add an element into elmts to be rendered
    private void AddCategoryElmt()
    {
        CategoryData newData = new();
        foreach (var elmt in elmts)
        {
            if (categories.Contains(elmt.Type)) categories.Remove(elmt.Type);
        }
        try
        {
            if (categories.Count < 1) throw new Exception("You cannot add more categories.");
        }
        catch (Exception e)
        {
            err = e.Message;
            return;
        }

        newData.Type = categories[0];
        elmts.Add(newData);
    }

    // Overloaded function for element initialization on pageload, taking two params
    private void AddCategoryElmt(string type, decimal? amt)
    {
        CategoryData newData = new CategoryData{
            Type = type,
            Amt = (decimal)amt
        };
        if (categories.Contains(newData.Type)) categories.Remove(newData.Type);
        try
        {
            if (categories.Count < 1) throw new Exception("You cannot add more categories.");
        }
        catch (Exception e)
        {
            err = e.Message;
            return;
        }
        elmts.Add(newData);
    }

    // Callback function to remove element (see CategoryElmt.razor)
    private void RemoveCategoryElmt(CategoryData del)
    {
        foreach (var elmt in elmts)
        {
            try
            {
                if (elmts.Count == 1) throw new Exception("You must have at least one category.");
            }
            catch (Exception e)
            {
                err = e.Message;
                return;
            }

            if (elmt.Equals(del))
            {
                elmts.Remove(elmt);
                categories.Add(elmt.Type);
                break;
            }
        }
    }

    // TODO: send elmts.CategoryData and BdgtAmt to db
    private async void SaveChangesAsync()
    {
        Type bdgttype = bdgt.GetType();
        PropertyInfo[] properties = bdgttype.GetProperties();

        foreach (PropertyInfo property in properties){

            switch (property.Name){
                case "HousingAmt":
                    if (!categories.Contains("Housing")){
                        var data = elmts.Find(x => x.Type == "Housing");
                        bdgt.HousingAmt = data.Amt;
                    }
                    else  {
                        bdgt.HousingAmt = null;
                    }
                    break;
                case "BillsUtilsAmt":
                    if (!categories.Contains("Bills/Utilities")){
                        var data = elmts.Find(x => x.Type == "Bills/Utilities");
                        bdgt.BillsUtilsAmt = data.Amt;
                    }
                    else  {
                        bdgt.BillsUtilsAmt = null;
                    }
                    break;
                case "GroceryDiningAmt":
                    if (!categories.Contains("Grocery/Dining")){
                        var data = elmts.Find(x => x.Type == "Grocery/Dining");
                        bdgt.GroceryDiningAmt = data.Amt;
                    }
                    else  {
                        bdgt.GroceryDiningAmt = null;
                    }
                    break;
                case "TransportAmt":
                    if (!categories.Contains("Transportation")){
                        var data = elmts.Find(x => x.Type == "Transportation");
                        bdgt.TransportAmt = data.Amt;
                    }
                    else  {
                        bdgt.TransportAmt = null;
                    }
                    break;
                case "EducationAmt":
                    if (!categories.Contains("Education")){
                        var data = elmts.Find(x => x.Type == "Education");
                        bdgt.EducationAmt = data.Amt;
                    }
                    else  {
                        bdgt.EducationAmt = null;
                    }
                    break;
                case "DebtAmt":
                    if (!categories.Contains("Debt")){
                        var data = elmts.Find(x => x.Type == "Debt");
                        bdgt.DebtAmt = data.Amt;
                    }
                    else  {
                        bdgt.DebtAmt = null;
                    }
                    break;
                case "EntertainmentAmt":
                    if (!categories.Contains("Entertainment")){
                        var data = elmts.Find(x => x.Type == "Entertainment");
                        bdgt.EntertainmentAmt = data.Amt;
                    }
                    else  {
                        bdgt.EntertainmentAmt = null;
                    }
                    break;
                case "ShoppingAmt":
                    if (!categories.Contains("Shopping")){
                        var data = elmts.Find(x => x.Type == "Shopping");
                        bdgt.ShoppingAmt = data.Amt;
                    }
                    else  {
                        bdgt.ShoppingAmt = null;
                    }
                    break;
                case "MedicalAmt":
                    if (!categories.Contains("Medical")){
                        var data = elmts.Find(x => x.Type == "Medical");
                        bdgt.MedicalAmt = data.Amt;
                    }
                    else  {
                        bdgt.MedicalAmt = null;
                    }
                    break;
                case "InvestingAmt":
                    if (!categories.Contains("Investing")){
                        var data = elmts.Find(x => x.Type == "Investing");
                        bdgt.InvestingAmt = data.Amt;
                    }
                    else  {
                        bdgt.InvestingAmt = null;
                    }
                    break;
                case "MiscAmt":
                    if (!categories.Contains("Miscellaneous")){
                        var data = elmts.Find(x => x.Type == "Miscellaneous");
                        bdgt.MiscAmt = data.Amt;
                    }
                    else  {
                        bdgt.MiscAmt = null;
                    }
                    break;
                default:
                    break;
            }
        }
        try{
            var updateResult = await dbContext.SaveChangesAsync();
        }
        catch(Exception e){
            err = e.Message;
        }
        msg = "Your changes have been saved.";
    }

    // Clears error out when alert is closed
    private void ClearErr()
    {
        err = string.Empty;
    }
    // Clear message out when alert is closed
    private void ClearMsg()
    {
        msg = string.Empty;
    }
}