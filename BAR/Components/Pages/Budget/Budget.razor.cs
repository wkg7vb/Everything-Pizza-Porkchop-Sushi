using BAR.Components.Pages.Budget.Modules;

namespace BAR.Components.Pages.Budget;

public partial class Budget
{
    // Vars
    private decimal BdgtAmt { get; set; } = 0;
    // TODO: pull user's settings to get their locale (currency type)
    private string userCurrencyLocale = "en-US";
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
    protected override void OnInitialized()
    {
        // TODO: pull user data from db and populate elmts w/ CategoryData objs
        if (false == true)
        {
            return;
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
    private void SaveChanges()
    {
        return;
    }

    // Clears error message out when alert is closed
    private void ClearErr()
    {
        err = string.Empty;
    }
}