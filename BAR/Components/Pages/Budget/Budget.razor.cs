using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using BAR.Components.Pages.Budget.Modules;
using BAR.Data.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BAR.Components.Pages.Budget;

public partial class Budget
{
    // User vars from db (init'd inside OnInitialiedAsync)
    private UserBudget? bdgt;
    private ApplicationUser? user {get; set;} = default!;

    [Inject] ToastService toastService {get; set;} = default!;

    // Local vars
    private string userCurrencyLocale {get; set;} = "";
    private List<CategoryData> elmts = new();
    private readonly Dictionary<string, string> categoryColumnNames = new Dictionary<string, string>{
        {"Housing", "HousingAmt"},
        {"Bills/Utilities", "BillsUtilsAmt"},
        {"Grocery/Dining", "GroceryDiningAmt"},
        {"Transportation", "TransportAmt"},
        {"Education", "EducationAmt"},
        {"Debt", "DebtAmt"},
        {"Entertainment", "EntertainmentAmt"},
        {"Shopping", "ShoppingAmt"},
        {"Medical", "MedicalAmt"},
        {"Investing", "InvestingAmt"},
        {"Miscellaneous", "MiscAmt"}
    };
    private List<string> categories = new();


    // Functions
    // Initialize local categories list
    protected override void OnInitialized()
    {
        base.OnInitialized();
        categories = categoryColumnNames.Keys.ToList();
    }

    // Runs when page is loaded, pseudo "constructor"-like function to pull data and fill page with respective elements
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity!.IsAuthenticated){
            user = await UserManager.GetUserAsync(authState.User);
        }
        if (user is not null)
        {
            userCurrencyLocale = user.UserLocale;
            bdgt = await dbContext.UserBudgets.SingleOrDefaultAsync(p => p.UserId == user.Id);

            if (bdgt is not null)
            {
                if (bdgt.HousingAmt is not null) AddCategoryElmt(type: "Housing", amt: (decimal)bdgt.HousingAmt);
                if (bdgt.BillsUtilsAmt is not null) AddCategoryElmt(type: "Bills/Utilities", amt: (decimal)bdgt.BillsUtilsAmt);
                if (bdgt.GroceryDiningAmt is not null) AddCategoryElmt(type: "Grocery/Dining", amt: (decimal)bdgt.GroceryDiningAmt);
                if (bdgt.TransportAmt is not null) AddCategoryElmt(type: "Transportation", amt: (decimal)bdgt.TransportAmt);            
                if (bdgt.EducationAmt is not null) AddCategoryElmt(type: "Education", amt: (decimal)bdgt.EducationAmt);
                if (bdgt.DebtAmt is not null) AddCategoryElmt(type: "Debt", amt: (decimal)bdgt.DebtAmt);
                if (bdgt.EntertainmentAmt is not null) AddCategoryElmt(type: "Entertainment", amt: (decimal)bdgt.EntertainmentAmt);
                if (bdgt.ShoppingAmt is not null) AddCategoryElmt(type: "Shopping", amt: (decimal)bdgt.ShoppingAmt);
                if (bdgt.MedicalAmt is not null) AddCategoryElmt(type: "Medical", amt: (decimal)bdgt.MedicalAmt);
                if (bdgt.InvestingAmt is not null) AddCategoryElmt(type: "Investing", amt: (decimal)bdgt.InvestingAmt);
                if (bdgt.MiscAmt is not null) AddCategoryElmt(type: "Miscellaneous", amt: (decimal)bdgt.MiscAmt);
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
        else try { throw new Exception("Invalid user."); }
        catch {
            NavigationManager.NavigateTo("/Account/Login");
        }
    }

    // Add an element into elmts to be rendered
    private void AddCategoryElmt()
    {
        UpdateCategories();
        CategoryData newData = new CategoryData{
            Type = categories[0],
            Amt = 0.0m
        };
        try
        {
            if (categories.Count < 1) throw new Exception("You cannot add more categories.");
        }
        catch (Exception e)
        {
            toastService.Notify(
                new ToastMessage(ToastType.Danger, e.Message)
            );
            return;
        }
        elmts.Add(newData);
        UpdateCategories();
    }

    // Overloaded function for element initialization on pageload, taking two params
    private void AddCategoryElmt(string type, decimal amt)
    {
        CategoryData newData = new CategoryData{
            Type = type,
            Amt = amt
        };
        try
        {
            if (categories.Count < 1) throw new Exception("You cannot add more categories.");
        }
        catch (Exception e)
        {
            toastService.Notify(
                new ToastMessage(ToastType.Danger, e.Message)
            );
            return;
        }
        elmts.Add(newData);
        UpdateCategories();
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
                toastService.Notify(
                    new ToastMessage(ToastType.Danger, e.Message)
                );
                return;
            }

            if (elmt.Equals(del))
            {
                elmts.Remove(elmt);
                UpdateCategories();
                break;
            }
        }
    }

    // Update categories list to reflect what categories are in use and which aren't
    private void UpdateCategories(){
        categories = categoryColumnNames.Keys.ToList();
        foreach (var col in elmts){
            if (categories.Contains(col.Type)){
                categories.Remove(col.Type);
            }
        }
    }

    // Saves the user's input data to their UserBudget in the db
    private async void SaveChangesAsync()
    {
        UpdateCategories();
        Type bdgttype = bdgt!.GetType();
        PropertyInfo[] properties = bdgttype.GetProperties();

        foreach (PropertyInfo property in properties){
            switch (property.Name){
                case "BudgetId":
                case "UserId":
                case "User":
                case "AllCategoriesNull":
                case "MonthlyIncome":
                    break;
                default:
                    bool shouldBreak = false;
                    foreach (var cat in categories){
                        if (categoryColumnNames[cat] == property.Name){
                            property.SetValue(bdgt, null);
                            shouldBreak = true;
                            break;
                        }
                    }
                    if (shouldBreak) break;
                    var data = elmts.Find(x => categoryColumnNames[x.Type] == property.Name);
                    property.SetValue(bdgt, data!.Amt);
                    break;
            }
        }
        try {
            var updateResult = await dbContext.SaveChangesAsync();
        }
        catch {
            toastService.Notify(
                new ToastMessage(ToastType.Danger, "An error occurred while saving your changes.")
            );
        }
        toastService.Notify(
                new ToastMessage(ToastType.Success, "Your changes have been saved.")
            );
    }

    private Task OnCategoryDataChange()
    {
        UpdateCategories();
        StateHasChanged();
        return Task.CompletedTask;
    }
}