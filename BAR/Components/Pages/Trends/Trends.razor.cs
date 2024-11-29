using System.Security.Claims;
using BAR.Data;
using BAR.Data.Models;
using BAR.Data.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BAR.Components.Pages.Trends
{
    public partial class Trends
    {

        [Inject]
        private ApplicationDbContext DbContext { get; set; } = default!;

        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        private ITransaction TransactionManager { get; set; } = default!;

        private ApplicationUser user = default!;

        //recent transactions list
        private IEnumerable<UserTransaction>? transactions;

        // monthly total chart
        private LineChart totalByMonth = default!;
        private LineChartOptions lineChartOptions = default!;
        // Data for the chart (labels and datasets)
        private ChartData totalByMonthData = default!;

        // bar chart variables
        // avg by category - NOT WORKING
        private BarChart barChart = default!;
        private BarChartOptions barChartOptions = default!;
        private ChartData chartData = default!;

        private Dictionary<int, string> months = new Dictionary<int, string> {
            {1, "January"},
            {2, "February"},
            {3, "March"},
            {4, "April"},
            {5, "May"},
            {6, "June"},
            {7, "July"},
            {8, "August"},
            {9, "September"},
            {10, "October"},
            {11, "November"},
            {12, "December"}
            };

        private Dictionary<int, string> categories = new Dictionary<int, string> {
            {1, "Housing"},
            {2, "Bills/Utilities"},
            {3, "Grocery/Dining"},
            {4, "Transportation"},
            {5, "Education"},
            {6, "Debt"},
            {7, "Entertainment"},
            {8, "Shopping"},
            {9, "Medical"},
            {10, "Investing"},
            {11, "Miscellaneous"}
            };

        // total by month task
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                user = await UserManager.GetUserAsync(authState.User);
            }
            else throw new Exception("Invalid user.");

            // total by month graph
            // Initialize chart data with 6 labels (past 6 months) and 4 datasets (Bills, Education, Debt, Investing)
            var labels = months.Values.ToList();
            totalByMonthData = new ChartData
            {
                Labels = labels[(DateTime.Now.Month - 6)..(DateTime.Now.Month)],
                Datasets = GetTotalAmountData()
            };
            // Configure the chart to be responsive and set interaction mode to 'index'
            lineChartOptions = new() { Responsive = true, Interaction = new Interaction { Mode = InteractionMode.Index } };
            // Set the color of X-axis and Y-axis ticks
            lineChartOptions.Scales.X!.Ticks = new ChartAxesTicks { Color = "blue" }; // X-axis ticks color
            lineChartOptions.Scales.Y!.Ticks = new ChartAxesTicks { Color = "green" }; // Y-axis ticks color

        }

        // Monthly Projection
        private LineChart lineChart = default!;
        private LineChartOptions lineChartOptions1 = default!;
        private ChartData chartData3 = default!;

        protected override void OnInitialized()
        {
            var colors = ColorUtility.CategoricalTwelveColors;

            // day 1, day 2, etc
            var labels = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            var datasets = new List<IChartDataset>();

            var dataset1 = new LineChartDataset
            {
                Label = "Projection from Last Month",
                Data = new List<double?> { 7265791, 5899643, 6317759, 6315641, 5338211, 8496306, 7568556, 8538933, 8274297, 8657298, 7548388, 7764845 },
                BackgroundColor = colors[0],
                BorderColor = colors[0],
                BorderWidth = 2,
                HoverBorderWidth = 4,
                // PointBackgroundColor = colors[0],
                // PointRadius = 0, // hide points
                // PointHoverRadius = 4,
            };
            datasets.Add(dataset1);

            var dataset2 = new LineChartDataset
            {
                Label = "Curent Spending per Day",
                Data = new List<double?> { 1809499, 1816642, 2122410, 1809499, 1850793, 1846743, 1954797, 2391313, 1983430, 2469918, 2633303, 2821149 },
                BackgroundColor = colors[1],
                BorderColor = colors[1],
                BorderWidth = 2,
                HoverBorderWidth = 4,
                // PointBackgroundColor = colors[1],
                // PointRadius = 0, // hide points
                // PointHoverRadius = 4,
            };
            datasets.Add(dataset2);

            chartData3 = new ChartData { Labels = labels, Datasets = datasets };

            lineChartOptions1 = new();
            lineChartOptions1.Responsive = true;
            lineChartOptions1.Interaction = new Interaction { Mode = InteractionMode.Index };

            lineChartOptions1.Scales.X!.Title = new ChartAxesTitle { Text = "Day", Display = true };
            lineChartOptions1.Scales.Y!.Title = new ChartAxesTitle { Text = "Total Spending", Display = true };

            lineChartOptions1.Plugins.Title!.Text = "Total Spending by Day Compared to Last Month";
            lineChartOptions1.Plugins.Title.Display = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                await totalByMonth.InitializeAsync(totalByMonthData, lineChartOptions1);

                // BAR CHART NOT WORKING
                //await barChart.InitializeAsync(chartData, barChartOptions);

                await lineChart.InitializeAsync(chartData3, lineChartOptions1);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        //generate data for the 4 categories: Bills, Education, Debt, and Investing
        private List<IChartDataset> GetTotalAmountData()
        {
            var datasets = new List<IChartDataset>
            {
                new LineChartDataset
                {
                    Label = "Total Amount Spent",
                    Data = GetMonthlyDataPoints(),
                    BackgroundColor = "rgba(255, 99, 132, 0.5)",  // Light red
                    BorderColor = "rgba(255, 99, 132, 1)",        // Red border
                    BorderWidth = 2
                },
            };
            return datasets;
        }

        //get monthly spending totals 
        private List<double?> GetMonthlyDataPoints()
        {
            var data = new List<double?>();
            // Generate random spending amounts for the 6-month period (Jan - June)
            for (var month = 0; month < 6; month++)
            {
                // Fetch user's transactions for the current month
                var monthBehind = DateTime.Now.Month - month;
                var transactions = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id && t.TransactionDateTime.Month == monthBehind)
                    .ToList();

                // Calculate the total spent in the current month
                var monthlyTotalSpent = transactions.Sum(t => t.TransactionAmt);
                data.Insert(0, (double)monthlyTotalSpent);
            }
            return data;
        }

        // Data provider for the Grid component displaying transactions
        private async Task<GridDataProviderResult<UserTransaction>> TransactionsDataProvider(GridDataProviderRequest<UserTransaction> request)
        {
            if (transactions == null) // Fetch transactions only once to optimize
                transactions = await GetUserTransactionsAsync();

            return await Task.FromResult(request.ApplyTo(transactions));
        }

        // Fetch the biggest 3 transactions
        private async Task<IEnumerable<UserTransaction>> GetUserTransactionsAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Enumerable.Empty<UserTransaction>();

            // Get the biggest 3 transactions for the current user, ordered by date
            return await DbContext.UserTransactions
                .Where(t => t.UserId == userId && t.TransactionDateTime.Month == DateTime.Now.Month)
                .OrderByDescending(t => t.TransactionAmt)
                .Take(3)
                .ToListAsync();
        }

        protected override void OnInitialized()
        {
            var colors = ColorUtility.CategoricalTwelveColors;

            var labels = categories.Values.ToList();
            var datasets = new List<IChartDataset>();

            // average spent bar
            var dataset1 = new BarChartDataset()
            {
                Data = GetAverageDataPoints(),
                BackgroundColor = new List<string> { colors[0] },
                BorderColor = new List<string> { colors[0] },
                BorderWidth = new List<double> { 0 },
            };
            datasets.Add(dataset1);

            chartData = new ChartData { Labels = labels, Datasets = datasets };

            barChartOptions = new();
            barChartOptions.Locale = "de-DE";
            barChartOptions.Responsive = true;
            barChartOptions.Interaction = new Interaction { Mode = InteractionMode.Y };
            barChartOptions.IndexAxis = "y";

            barChartOptions.Scales.X!.Title = new ChartAxesTitle { Text = "Amount", Display = true };
            barChartOptions.Scales.Y!.Title = new ChartAxesTitle { Text = "Categories", Display = true };

            barChartOptions.Scales.X.Stacked = true;
            barChartOptions.Scales.Y.Stacked = true;

            barChartOptions.Plugins.Title!.Text = "6 Month Averages by Category";
            barChartOptions.Plugins.Title.Display = true;
        }

        //get category spending totals & averages 
        private List<double?> GetAverageDataPoints()
        {
            var data = new List<double?>();
            var counter = new List<int?>(11);

            // getting spending amounts for past 6 months
            for (var month = 0; month < 6; month++)
            {
                // Fetch user's transactions for the current month iteration
                var monthBehind = DateTime.Now.Month - month;
                foreach (var item in categories)
                { // going through category dictionary
                    var transactions = DbContext.UserTransactions // not too sure
                        .Where(t => t.UserId == user.Id && t.TransactionCategory == item.Value) // match the category
                        .ToList();
                    counter[item.Key] = counter[item.Key] + 1; // count each category
                }

                foreach (var item in categories)
                {
                    // Calculate the average by category
                    var avgByCategory = transactions.Sum(t => t.TransactionAmt) / counter[item.Key];

                    // put info into data 
                    data.Insert(0, (double)avgByCategory);
                }

            }
            return data;
        }
    }
}