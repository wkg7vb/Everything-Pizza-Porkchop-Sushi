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

        private ApplicationUser user = default!;

        //recent transactions list (transaction list)
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

        // Monthly Projection variables
        private LineChart projectionsChart = default!;
        private LineChartOptions lineChartOptions1 = default!;
        private ChartData projectionsChartData = default!;

        private readonly Dictionary<int, string> months = new() {
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

        private readonly Dictionary<int, string> categories = new() {
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

        // Returns the correct month and year behind as ints given a number of months
        // Max use case is 5 so never need to go back more than one year, thus its hardcoded
        private static int[] GetMonthBehind(int amt_to_go_back){
            if (DateTime.Now.Month - amt_to_go_back < 1){
                return [12 + (DateTime.Now.Month - amt_to_go_back), DateTime.Now.Year - 1];
            }
            else return [DateTime.Now.Month - amt_to_go_back, DateTime.Now.Year];
        }

        // OnInit functionality
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(10);
            await base.OnInitializedAsync();

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity!.IsAuthenticated)
            {
                user = await UserManager.GetUserAsync(authState.User);
            }
            else throw new Exception("Invalid user.");

            // total by month graph
            // Initialize chart data with 6 labels (past 6 months) and 4 datasets (Bills, Education, Debt, Investing)
            List<string> labels = [];  // putting months onto label value
            for (int i = 0; i < 6; i++){
                int[] behind = GetMonthBehind(i);
                labels.Insert(0,months[behind[0]]);
            }

            totalByMonthData = new ChartData
            {
                // calculating past 6 months
                Labels = labels,
                Datasets = GetTotalAmountData()
            };

            // Configure the chart to be responsive and set interaction mode to 'index'
            lineChartOptions = new()
            {
                Responsive = true,
                Interaction = new Interaction { Mode = InteractionMode.Index }
            };

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
            await Task.Delay(100);
            if (firstRender)
            {
                await totalByMonth.InitializeAsync(totalByMonthData, lineChartOptions);
                await avgsBarChart.InitializeAsync(avgsBarChartData, avgsBarChartOptions);
                await projectionsChart.InitializeAsync(projectionsChartData, lineChartOptions1);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        // generate data for totals (line graph 1)
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

        // get monthly spending totals (line graph 1) 
        private List<double?> GetMonthlyDataPoints()
        {
            var data = new List<double?>();
            // Generate random spending amounts for the 6-month period (Jan - June)
            for (var month = 0; month < 6; month++)
            {
                // Fetch user's transactions for the current month
                int[] behind = GetMonthBehind(month);
                var transactions = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id 
                    && t.TransactionDateTime.Month == behind[0]
                    && t.TransactionDateTime.Year == behind[1])
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
            await Task.Delay(50);
            transactions ??= await GetUserTransactionsAsync();
            return await Task.FromResult(request.ApplyTo(transactions));
        }
        private async Task<IEnumerable<UserTransaction>> GetUserTransactionsAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return [];

            // Get the biggest 3 transactions for the current user, ordered by date
            return await DbContext.UserTransactions
                .Where(t => t.UserId == userId 
                        && t.TransactionDateTime.Month == DateTime.Now.Month 
                        && t.TransactionDateTime.Year == DateTime.Now.Year)
                .OrderByDescending(t => t.TransactionAmt)
                .Take(3)
                .ToListAsync();
        }

        // bar chart stuff?

        //get category spending totals & averages 
        private List<double?> GetAverageDataPoints()
        {
            var data = new List<double?>();
            IEnumerable<UserTransaction> transactions_list = [];

            // getting spending amounts for past 6 months
            for (int month = 0; month < 6; month++){
                int[] behind = GetMonthBehind(month);  
                var tl = DbContext.UserTransactions
                        .Where(t => t.UserId == user.Id 
                            && t.TransactionDateTime.Month == behind[0]
                            && t.TransactionDateTime.Year == behind[1])
                        .ToList();
                transactions_list = transactions_list.Concat(tl);
            }
            
            if (!transactions_list.Any()) {
                return data;
            }

            foreach (var item in categories)
            {
                if (transactions_list.Any(t => t.TransactionCategory == item.Value) == false){
                    data.Add(0.00);
                    continue;
                }
                // Calculate the average by category
                var avgByCategory = transactions_list
                    .Where(t => t.TransactionCategory == item.Value)
                    .Sum(t => t.TransactionAmt) /
                    transactions_list.Count(t => t.TransactionCategory == item.Value);

                // put info into data 
                data.Add(Math.Round((double)avgByCategory,2));
            }
            return data;
        }


        //get last month's spending totals (line graph 2) 
        private List<double?> GetLastMonthDataPoints()
        {
            var data = new List<double?>();
            int[] behind = GetMonthBehind(1);
            var transactions_list = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id 
                            && t.TransactionDateTime.Month == behind[0]
                            && t.TransactionDateTime.Year == behind[1])
                    .ToList();
            var monthlySlope = transactions_list.Sum(t => t.TransactionAmt) / DateTime.DaysInMonth(behind[1], behind[0]);

            for (int i = 1; i < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + 1; i++){
                double count = i * (double)monthlySlope;
                data.Add(Math.Round(count,2));
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
                var transactions_list = DbContext.UserTransactions
                    .Where(t => t.UserId == user.Id 
                            && t.TransactionDateTime.Day == i 
                            && t.TransactionDateTime.Month == DateTime.Now.Month
                            && t.TransactionDateTime.Year == DateTime.Now.Year)
                    .ToList();

                // Calculate the total spent in the current month
                var currentDaySum = transactions_list.Sum(t => t.TransactionAmt);
                total += (double)currentDaySum;
                data.Add(total);
            }

            return data;
        }
    } 
}