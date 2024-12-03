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
        private BarChart avgsBarChart = default!;
        private BarChartOptions avgsBarChartOptions = default!;
        private ChartData avgsBarChartData = default!;

        // Monthly Projection
        private LineChart projectionsChart = default!;
        private LineChartOptions lineChartOptions1 = default!;
        private ChartData projectionsChartData = default!;
        int pastDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1); // error 
        int currentDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

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
        private static int year;
        private static int month;

        private int GetMonthBehind(int amt_to_go_back){
            if (DateTime.Now.Month - amt_to_go_back < 1){
                return 12 + (DateTime.Now.Month - amt_to_go_back);
            }
            else return DateTime.Now.Month - amt_to_go_back;
        }

        // total by month task
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity!.IsAuthenticated)
            {
                user = await UserManager.GetUserAsync(authState.User);
            }
            else throw new Exception("Invalid user.");

            // total by month graph
            // Initialize chart data with 6 labels (past 6 months) and 4 datasets (Bills, Education, Debt, Investing)
            var labels = months.Values.ToList(); // putting months onto label value

            totalByMonthData = new ChartData
            {
                // calculating past 6 months
                Labels = labels[(DateTime.Now.Month - 6)..(DateTime.Now.Month)],
                Datasets = GetTotalAmountData()
            };
            // Configure the chart to be responsive and set interaction mode to 'index'
            lineChartOptions = new() { Responsive = true, Interaction = new Interaction { Mode = InteractionMode.Index } };
            // Set the color of X-axis and Y-axis ticks
            lineChartOptions.Scales.X!.Ticks = new ChartAxesTicks { Color = "blue" }; // X-axis ticks color
            lineChartOptions.Scales.Y!.Ticks = new ChartAxesTicks { Color = "green" }; // Y-axis ticks color
            
            lineChartOptions.Scales.X!.Title = new ChartAxesTitle { Text = "Month", Display = true };
            lineChartOptions.Scales.Y!.Title = new ChartAxesTitle { Text = "Total Spending by Month", Display = true };
            lineChartOptions.Plugins.Title!.Text = "Total Monthly Spending of the Past 6 Months";
            lineChartOptions.Plugins.Title.Display = true;


            // PROJECTIONS 
            var colors = ColorUtility.CategoricalTwelveColors;

            // day 1, day 2, etc - get current month total days
            // list with this month's days on it
            List<string> labels_projection = new();

            for (int i = 1; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1; i++){
                string Mo = months[DateTime.Now.Month];
                labels_projection.Add($"{Mo} {i}");
            }
            

            // labelsp has to be a string
            var projectionsDatasets = new List<IChartDataset>();

            // line graph from last month (slope)
            var dataset1 = new LineChartDataset
            {
                Label = "Projection from Last Month",
                Data = GetLastMonthDataPoints(),
                BackgroundColor = colors[0],
                BorderColor = colors[0],
                BorderWidth = 2,
                HoverBorderWidth = 4,
            };
            projectionsDatasets.Add(dataset1);

            // daily spending from this month (current data)
            var dataset2 = new LineChartDataset
            {
                Label = "Curent Spending per Day",
                Data = GetMonthsDayDataPoints(),
                BackgroundColor = colors[1],
                BorderColor = colors[1],
                BorderWidth = 2,
                HoverBorderWidth = 4,
            };
            projectionsDatasets.Add(dataset2);

            projectionsChartData = new ChartData { Labels = labels_projection, Datasets = projectionsDatasets };

            lineChartOptions1 = new()
            {
                Responsive = true,
                Interaction = new Interaction { Mode = InteractionMode.Index }
            };

            lineChartOptions1.Scales.X!.Title = new ChartAxesTitle { Text = "Day", Display = true };
            lineChartOptions1.Scales.Y!.Title = new ChartAxesTitle { Text = "Total Spending", Display = true };
            lineChartOptions1.Plugins.Title!.Text = "Total Spending by Day Compared to Last Month";
            lineChartOptions1.Plugins.Title.Display = true;


            // BAR CHART
            var labels1 = categories.Values.ToList();
            var barChartDatasets = new List<IChartDataset>();

            // average spent bar
            var dataset1b = new BarChartDataset()
            {
                Data = GetAverageDataPoints(),
                BackgroundColor = new List<string> { colors[0] },
                BorderColor = new List<string> { colors[0] },
                BorderWidth = new List<double> { 0 },
                Label = "Average Spent in Category",
            };
            barChartDatasets.Add(dataset1b);

            avgsBarChartData = new ChartData { Labels = labels1, Datasets = barChartDatasets };

            avgsBarChartOptions = new()
            {
                Locale = user!.UserLocale,
                Responsive = true,
                Interaction = new Interaction { Mode = InteractionMode.X },
                IndexAxis = "x"
            };

            avgsBarChartOptions.Scales.Y!.Title = new ChartAxesTitle { Text = "Amount", Display = true };
            avgsBarChartOptions.Scales.X!.Title = new ChartAxesTitle { Text = "Categories", Display = true };
            avgsBarChartOptions.Scales.X.Stacked = true;
            avgsBarChartOptions.Scales.Y.Stacked = true;
            avgsBarChartOptions.Plugins.Title!.Text = "6 Month Averages by Category";
            avgsBarChartOptions.Plugins.Title.Display = true;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender) // show chart
        {

            if (firstRender)
            {
                await totalByMonth.InitializeAsync(totalByMonthData, lineChartOptions1);
                await avgsBarChart.InitializeAsync(avgsBarChartData, avgsBarChartOptions);
                await projectionsChart.InitializeAsync(projectionsChartData, lineChartOptions1);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        //generate data for totals (line graph 1)
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

        //get monthly spending totals (line graph 1) 
        private List<double?> GetMonthlyDataPoints()
        {
            var data = new List<double?>();
            // Generate random spending amounts for the 6-month period (Jan - June)
            for (var month = 0; month < 6; month++)
            {
                // Fetch user's transactions for the current month
                var transactions = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id && t.TransactionDateTime.Month == GetMonthBehind(month))
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

        // bar chart stuff?

        //get category spending totals & averages 
        private List<double?> GetAverageDataPoints()
        {
            var data = new List<double?>();
            var counter = new int[11];

            // getting spending amounts for past 6 months
            var transactions = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id 
                        && t.TransactionDateTime.Month == GetMonthBehind(0)
                        || t.TransactionDateTime.Month == GetMonthBehind(1)
                        || t.TransactionDateTime.Month == GetMonthBehind(2)
                        || t.TransactionDateTime.Month == GetMonthBehind(3)
                        || t.TransactionDateTime.Month == GetMonthBehind(4)
                        || t.TransactionDateTime.Month == GetMonthBehind(5))
                    .ToList();
            
            if (transactions.Count() == 0) {
                return data;
            }

            foreach (var item in categories)
            {
                if (transactions.Count(t => t.TransactionCategory == item.Value) == 0){
                    data.Add(0.00);
                    continue;
                }
                // Calculate the average by category
                var avgByCategory = transactions
                    .Where(t => t.TransactionCategory == item.Value)
                    .Sum(t => t.TransactionAmt) /
                    transactions.Count(t => t.TransactionCategory == item.Value);

                // put info into data 
                data.Add(Math.Round((double)avgByCategory,2));
            }
            return data;
        }


        //get last month's spending totals (line graph 2) 
        private List<double?> GetLastMonthDataPoints()
        {
            var data = new List<double?>();

            var transactions = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id && t.TransactionDateTime.Month == GetMonthBehind(1))
                    .ToList();
            var monthlySlope = transactions.Sum(t => t.TransactionAmt) / DateTime.DaysInMonth(DateTime.Now.Year, GetMonthBehind(1));


            for (int i = 1; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1; i++){
                double count = i * (double)monthlySlope;
                data.Add(count);
            }

            return data;
        }

        //get this month's daily spending totals (line graph 2) 
        private List<double?> GetMonthsDayDataPoints()
        {
            var data = new List<double?>();
            double total = 0.0;
           
            for (int i = 1; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1; i++)
            {
                // Fetch user's transactions for the iterated day
                var transactions = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id && t.TransactionDateTime.Day == i && t.TransactionDateTime.Month == DateTime.Now.Month)
                    .ToList();

                // Calculate the total spent in the current month
                var currentDaySum = transactions.Sum(t => t.TransactionAmt);
                total = total + (double)currentDaySum;
                data.Add(total);
            }

            return data;
        }
    } 
}