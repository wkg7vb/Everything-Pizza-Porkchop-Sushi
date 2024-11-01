using System.Net.Http.Headers;
using BAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using BlazorBootstrap;
using BAR.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace BAR.Components.Pages.Homepage
{
    public partial class Homepage
    {
        [Inject]
        private ApplicationDbContext DbContext { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;


        private DoughnutChart doughnutChart = default!;
        private DoughnutChartOptions doughnutChartOptions = default!;

        // Data structure to hold chart data (labels and datasets)
        private ChartData chartData = default!;

        private string[] backgroundColors = ColorUtility.CategoricalTwelveColors;

        // Random number generator for generating random data
        private Random random = new();

        // Amounts for each category in both datasets
        private List<double?> dataset1Amounts = new();
        private List<double?> dataset2Amounts = new();

        private decimal monthlyBudgetTotal;

        protected override async Task OnInitializedAsync()
        {
            // Initialize the chart options
            doughnutChartOptions = new();
            doughnutChartOptions.Responsive = true; // Make the chart responsive

            // Set the title for the chart
            doughnutChartOptions.Plugins.Title!.Text = "Monthly Expenses";
            doughnutChartOptions.Plugins.Title.Display = true;

            // Generate random amounts for two datasets
            dataset1Amounts = GenerateRandomAmounts();
            dataset2Amounts = GenerateRandomAmounts();

            // Create chart data with labels and datasets
            chartData = new ChartData
            {
                Labels = new List<string> { "Bills", "Debt", "Investing", "Education" },
                Datasets = new List<IChartDataset>
            {
                new DoughnutChartDataset
                {
                    Label = "Amount Spent",
                    Data = dataset1Amounts,
                    BackgroundColor = new List<string>  //set colors for dataset
                    {
                        backgroundColors[0],
                        backgroundColors[1],
                        backgroundColors[2],
                        backgroundColors[3]
                    }
                },
                new DoughnutChartDataset
                {
                    Label = "Amount Left",
                    Data = dataset2Amounts,
                    BackgroundColor = new List<string>
                    {
                        backgroundColors[4],
                        backgroundColors[5],
                        backgroundColors[6],
                        backgroundColors[7]
                    }
                }
            }
            };
            randomMoneyAmount = $"${GetRandomMoneyAmount()}";  //initialize random money amount onto the cards
            await CalculateMonthlyBudgetTotal();
        }

        private async Task CalculateMonthlyBudgetTotal()
        {
            // Get the current user's authentication state
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Retrieve the user's ID
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var userBudget = await DbContext.UserBudgets
                    .Where(b => b.UserId == userId) // Filter by the current user
                    .FirstOrDefaultAsync();

                if (userBudget != null)
                {
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
                    monthlyBudgetTotal = 0; // Set to zero if no budget found
                }
            }
            else
            {
                monthlyBudgetTotal = 0; // Handle case where user ID is null
            }
        }

        // Method called after the component has rendered
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) // Ensure the initialization only occurs once
            {
                // Initialize the DoughnutChart with data and options
                await doughnutChart.InitializeAsync(chartData, doughnutChartOptions);
            }
            // Call the base class implementation
            await base.OnAfterRenderAsync(firstRender);
        }

        // Method to generate random amounts for each category
        private List<double?> GenerateRandomAmounts()
        {
            var amounts = new List<double?>();
            for (int i = 0; i < 4; i++) // Create four random amounts
            {
                // Generate a random dollar amount between 0.00 and 1000.00 and round to two decimal places
                double randomAmount = Math.Round(random.NextDouble() * 1000, 2);
                amounts.Add(randomAmount);
            }
            return amounts;
        }


        //Random money amount generator
        private string randomMoneyAmount = ".00";
        private string GetRandomMoneyAmount()
        {
            double randomAmount = Math.Round(random.NextDouble() * 1000, 2);
            return randomAmount.ToString("0.00");
        }


    }
}