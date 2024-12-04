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
    private ApplicationUser? User {get; set;} = default!;
    private Modal AddCatModal = default!;
    private string AddedCat = string.Empty;

    [Inject] ToastService ToastService {get; set;} = default!;

    // Local vars
    private string userCurrencyLocale {get; set;} = "";
    private List<CategoryData> elmts = [];
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
    private List<string> categories = [];


    // Functions
    // Initialize local categories list
    protected override void OnInitialized()
    {
        Task.Delay(10);
        base.OnInitialized();
        categories = [.. categoryColumnNames.Keys];
    }

    // Runs when page is loaded, pseudo "constructor"-like function to pull data and fill page with respective elements
    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(10);
        await base.OnInitializedAsync();
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity!.IsAuthenticated){
            User = await UserManager.GetUserAsync(authState.User);
        }
        if (User is not null)
        {
            userCurrencyLocale = User.UserLocale;
            bdgt = await dbContext.UserBudgets.SingleOrDefaultAsync(p => p.UserId == User.Id);

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
                if (elmts.Count == 0 && bdgt.AllCategoriesNull) AddCategoryElmt(categories[0], 0.0m);
            }
            // Initialize a generic category cell when no user data is available
            else
            {
                bdgt = new UserBudget{
                    UserId = User.Id,
                    User = User
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

    // Create a category element given a type and anount
    private void AddCategoryElmt(string type, decimal amt)
    {
        CategoryData newData = new()
        {
            Type = type,
            Amt = amt
        };
        try
        {
            if (categories.Count < 1) throw new Exception("You cannot add more categories.");
        }
        catch (Exception e)
        {
            ToastService.Notify(
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
                ToastService.Notify(
                    new ToastMessage(ToastType.Danger, e.Message)
                );
                return;
            }

            if (elmt.Equals(del))
            {
                elmts.Remove(elmt);
                UpdateCategories();
                return;
            }
        }
    }

    // Update categories list to reflect what categories are in use and which aren't
    private void UpdateCategories(){
        categories = [.. categoryColumnNames.Keys];
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
            ToastService.Notify(
                new ToastMessage(ToastType.Danger, "An error occurred while saving your changes.")
            );
        }
        ToastService.Notify(
                new ToastMessage(ToastType.Success, "Your changes have been saved.")
            );
    }

    // Show/hide modal based on user inputs
    private async Task ShowAddCatModal()
    {
        if (categories.Count == 0){
            ToastService.Notify(
                new ToastMessage(ToastType.Danger, "You have already added all categories.")
            );
            return;
        }
        await AddCatModal.ShowAsync();
    }
    private async Task CancelAddCatModal()
    {
        AddedCat = string.Empty;
        await AddCatModal.HideAsync();
    }

    // Callback to add provided category and update UI
    private async void AddCategoryFromModal(){
        if (AddedCat == string.Empty)
        {
            ToastService.Notify(
                new ToastMessage(ToastType.Danger, "Please select a category.")
            );
            return;
        }
        else
        {
            AddCategoryElmt(AddedCat, 0.0m);
        }
        AddedCat = string.Empty;
        await AddCatModal.HideAsync();
        ToastService.Notify(
                new ToastMessage(ToastType.Success, "New category added.")
            );
    }
}