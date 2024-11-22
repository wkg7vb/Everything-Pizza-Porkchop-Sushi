using System.Net.Http.Headers;
using BAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using BlazorBootstrap;
using BAR.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BAR.Components.Pages.Homepage
{
    public partial class Homepage
    {
        [Inject]
        private ApplicationDbContext DbContext { get; set; } = default!;

        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;


        private DoughnutChart doughnutChart = default!;
        private DoughnutChartOptions doughnutChartOptions = default!;

        // Data structure to hold chart data (labels and datasets)
        private ChartData chartData = default!;


        // Amounts for each category in both datasets
        private List<double?> dataset1Amounts = new();
        private List<double?> dataset2Amounts = new();

        //card vars
        private decimal monthlyBudgetTotal;
        private decimal monthlyIncome;
        private decimal monthlyTotalSpent = 0.00m;
        private decimal overMonthlyBudget = 0.00m;

        //user's first name vars
        private string userFirstName = "Partner";
        private string userLastName = "";

        //recent transactions list
        private IEnumerable<UserTransaction>? transactions;

        //semaphore to synchronize database calls
        private readonly SemaphoreSlim _dbSemaphore = new(1, 1);



        protected override async Task OnInitializedAsync()
        {
            // Initialize the chart options
            doughnutChartOptions = new();
            doughnutChartOptions.Responsive = true; // Make the chart responsive

            // Set the title for the chart
            doughnutChartOptions.Plugins.Title!.Text = "Monthly Expenses";
            doughnutChartOptions.Plugins.Title.Display = true;



            // Create chart data with labels and datasets
            // Load the actual data into dataset amounts
             await LoadChartDataAsync();
            

            // Create chart data with labels and datasets
            chartData = new ChartData
            {
                Labels = new List<string>
                {
                    "Housing", "Bills/Utilities", "Grocery/Dining", "Transportation",
                    "Education", "Debt", "Entertainment", "Shopping",
                    "Medical", "Investing", "Miscellaneous"
                },
                Datasets = new List<IChartDataset>
        {
            new DoughnutChartDataset
            {
                Label = "Amount Spent",
                Data = dataset1Amounts,
                BackgroundColor = new List<string> // Set specific colors for each category
            {
                "#0fb5ae",
                "#4046ca",
                "#f68511",
                "#de3d82",
                "#7e84fa",
                "#72e06a",
                "#147af3",
                "#7326d3",
                "#e8c600",
                "#cb5d00",
                "#008f5d"
            }
            },
            new DoughnutChartDataset
            {
                Label = "Amount Remaining",
                Data = dataset2Amounts,
                BackgroundColor = new List<string> // Set specific colors for each category
            {
                "#0fb5ae",
                "#4046ca",
                "#f68511",
                "#de3d82",
                "#7e84fa",
                "#72e06a",
                "#147af3",
                "#7326d3",
                "#e8c600",
                "#cb5d00",
                "#008f5d"
            }
            }
        }
            };

            
             await GetUserNames();
             await CalculateCardFinancials();
            
            
        }

        //method to get the current user's authentication state
        private async Task<string?> GetCurrentUserIdAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Data provider for the Grid component displaying list of recent transactions
        private async Task<GridDataProviderResult<UserTransaction>> TransactionsDataProvider(GridDataProviderRequest<UserTransaction> request)
        {
            if (transactions == null) // Fetch transactions only once to optimize
                transactions = await GetUserTransactionsAsync();

            return await Task.FromResult(request.ApplyTo(transactions));
        }

        // Fetch the recent transactions for the current user
        private async Task<IEnumerable<UserTransaction>> GetUserTransactionsAsync()
        {
            await _dbSemaphore.WaitAsync();

            try
            {
                var userId = await GetCurrentUserIdAsync();

                if (userId == null)
                    return Enumerable.Empty<UserTransaction>();

                // Get the latest 5 transactions for the current user, ordered by date
                return await DbContext.UserTransactions
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.TransactionDateTime)
                    .Take(5)
                    .ToListAsync();
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }


        //get the user's first/last name so the welcome screen displays their name.
        private async Task GetUserNames()
        {
            var userId = await GetCurrentUserIdAsync();

            if (userId != null)
            {
                // Fetch the user from the database
                var applicationUser = await UserManager.FindByIdAsync(userId);
                if (applicationUser != null)
                {
                    userFirstName = applicationUser.UserFName ?? "Partner"; // Use "Partner" if the first name is null
                    userLastName = applicationUser.UserLName ?? ""; // Use empty string if last name is null
                }
            }
        }
        //calculate the amounts for each card component
        private async Task CalculateCardFinancials()
        {
            await _dbSemaphore.WaitAsync();
            try
            {
                var userId = await GetCurrentUserIdAsync();

                if (userId != null)
                {
                    var userBudget = await DbContext.UserBudgets
                        .Where(b => b.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (userBudget != null)
                    {
                        monthlyIncome = userBudget.MonthlyIncome;
                        monthlyBudgetTotal =
                            (userBudget.HousingAmt ?? 0) +
                            (userBudget.BillsUtilsAmt ?? 0) +
                            (userBudget.GroceryDiningAmt ?? 0) +
                            (userBudget.TransportAmt ?? 0) +
                            (userBudget.EducationAmt ?? 0) +
                            (userBudget.DebtAmt ?? 0) +
                            (userBudget.EntertainmentAmt ?? 0) +
                            (userBudget.ShoppingAmt ?? 0) +
                            (userBudget.MedicalAmt ?? 0) +
                            (userBudget.InvestingAmt ?? 0) +
                            (userBudget.MiscAmt ?? 0);
                    }
                    else
                    {
                        monthlyBudgetTotal = 0;
                        monthlyIncome = 0;
                    }

                    // Fetch user's transactions for the current month
                    var transactions = await DbContext.UserTransactions
                        .Where(t => t.UserId == userId && t.TransactionDateTime.Month == DateTime.Now.Month)
                        .ToListAsync();

                    // Calculate the total spent in the current month
                    monthlyTotalSpent = transactions.Sum(t => t.TransactionAmt);

                    // Calculate if the user is over budget
                    overMonthlyBudget = monthlyTotalSpent > monthlyBudgetTotal ? monthlyTotalSpent - monthlyBudgetTotal : 0.00m;
                }
                else
                {
                    monthlyTotalSpent = 0.00m;
                    overMonthlyBudget = 0.00m;
                    monthlyBudgetTotal = 0;
                    monthlyIncome = 0;
                }
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }


        // Method called after the component has rendered
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Always initialize the DoughnutChart with data and options
            await doughnutChart.InitializeAsync(chartData, doughnutChartOptions);

            // Call the base class implementation
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task LoadChartDataAsync()
        {

            await _dbSemaphore.WaitAsync();
            try
            {
                var userId = await GetCurrentUserIdAsync();

                if (userId == null) return;

                // Fetch budget categories and amounts
                var budget = await DbContext.UserBudgets
                    .Where(b => b.UserId == userId)
                    .FirstOrDefaultAsync();

                if (budget != null)
                {
                    dataset2Amounts = new List<double?>
            {
                (double?)budget.HousingAmt,
                (double?)budget.BillsUtilsAmt,
                (double?)budget.GroceryDiningAmt,
                (double?)budget.TransportAmt,
                (double?)budget.EducationAmt,
                (double?)budget.DebtAmt,
                (double?)budget.EntertainmentAmt,
                (double?)budget.ShoppingAmt,
                (double?)budget.MedicalAmt,
                (double?)budget.InvestingAmt,
                (double?)budget.MiscAmt
            };
                }

                // Fetch recent transaction amounts by category
                var transactions = await DbContext.UserTransactions
                    .Where(t => t.UserId == userId)
                    .ToListAsync();

                // Aggregate transactions by category
                var transactionSums = transactions
                    .GroupBy(t => t.TransactionCategory)
                    .ToDictionary(
                        g => g.Key,
                        g => (double?)g.Sum(t => t.TransactionAmt)
                    );

                dataset1Amounts = new List<double?>
        {
            (double?)(transactionSums.GetValueOrDefault("Housing", 0)),
            (double?)(transactionSums.GetValueOrDefault("Bills/Utilities", 0)),
            (double?)(transactionSums.GetValueOrDefault("Grocery/Dining", 0)),
            (double?)(transactionSums.GetValueOrDefault("Transportation", 0)),
            (double?)(transactionSums.GetValueOrDefault("Education", 0)),
            (double?)(transactionSums.GetValueOrDefault("Debt", 0)),
            (double?)(transactionSums.GetValueOrDefault("Entertainment", 0)),
            (double?)(transactionSums.GetValueOrDefault("Shopping", 0)),
            (double?)(transactionSums.GetValueOrDefault("Medical", 0)),
            (double?)(transactionSums.GetValueOrDefault("Investing", 0)),
            (double?)(transactionSums.GetValueOrDefault("Misc", 0))
        };

                dataset2Amounts = dataset2Amounts
                    .Select((amount, index) => amount - dataset1Amounts[index])
                    .ToList();
            }
            finally
            {
                _dbSemaphore.Release();
            }
        }

        }
}