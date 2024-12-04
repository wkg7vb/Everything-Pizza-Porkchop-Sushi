using BAR.Data.Models;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BAR.Components.Pages.Transactions
{
    public partial class TransactionHistory
    {
        //vars
        private ApplicationUser user = default!;
        private List<UserTransaction> userTransactions = default!;
        private DateTime date = DateTime.Now;
        private string category = "Housing";
        private string label = "";
        private decimal amount = 0.00m;
        private UserTransaction transactionToUpdate = default!;
        private Grid<UserTransaction> transactionGrid = default!;

        //dependency injection
        [Inject] ToastService toastService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        //methods
        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(10);
            await base.OnInitializedAsync();
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                user = await UserManager.GetUserAsync(authState.User);
            }
        }

        //grid
        private async Task<GridDataProviderResult<UserTransaction>> TransactionDataProvider(GridDataProviderRequest<UserTransaction> request)
        {
            await Task.Delay(20);
            if (userTransactions == null)
            {
                userTransactions = await TransactionManager.GetTransactions(user.Id);
            }
            return await Task.FromResult(request.ApplyTo(userTransactions));
        }

        private async Task OnGridSettingsChanged(GridSettings settings)
        {
            if (settings is null)
                return;

            // NOTE: enable below two lines, if you want to set default values for PageNumber and PageSize all the time.
            //settings.PageNumber = 1;
            //settings.PageSize = 10;

            await JS.InvokeVoidAsync("window.localStorage.setItem", "grid-settings", JsonSerializer.Serialize(settings));
        }

        private async Task<GridSettings> GridSettingsProvider()
        {
            var settingsJson = await JS.InvokeAsync<string>("window.localStorage.getItem", "grid-settings");
            if (string.IsNullOrWhiteSpace(settingsJson))
                return null!;

            var settings = JsonSerializer.Deserialize<GridSettings>(settingsJson);
            if (settings is null)
                return null!;

            return settings;
        }

        //update modal
        private Modal updatemodal = default!;
        private async Task ShowUpdateModal(GridRowEventArgs<UserTransaction> transaction)
        {
            transactionToUpdate = transaction.Item;
            date = transactionToUpdate.TransactionDateTime;
            label = transactionToUpdate.TransactionLabel;
            amount = transactionToUpdate.TransactionAmt;
            category = transactionToUpdate.TransactionCategory;
            await updatemodal.ShowAsync();
        }

        private async Task CancelUpdateModal()
        {
            await updatemodal.HideAsync();
        }

        private async Task UpdateFromModal()
        {
            if (label != null && amount != 0.00m)
            {
                transactionToUpdate.TransactionDateTime = date;
                transactionToUpdate.TransactionLabel = label;
                transactionToUpdate.TransactionAmt = amount + 0.00m;
                transactionToUpdate.TransactionCategory = category;

                transactionToUpdate.UserId = user.Id;
                transactionToUpdate.User = user;

                await TransactionManager.UpdateTransaction(transactionToUpdate);
                await updatemodal.HideAsync();
                toastService.Notify(new ToastMessage(ToastType.Success, $"Successfully updated transaction."));
                NavigationManager.Refresh(true);
            }
            else
            {
                toastService.Notify(new ToastMessage(ToastType.Danger, $"Please fill all fields."));
            }
        }

        //delete dialog
        private ConfirmDialog deletedialog = default!;
        private async Task ShowDeleteConfirm()
        {
            var confirmation = await deletedialog.ShowAsync(
                title: "Delete Transaction?",
                message1: transactionToUpdate.TransactionLabel.ToString(),
                message2: transactionToUpdate.TransactionAmt.ToString());

            if (confirmation)
            {
                //delete transaction
                await TransactionManager.DeleteTransaction(transactionToUpdate);
                await updatemodal.HideAsync();
                toastService.Notify(new ToastMessage(ToastType.Success, $"Transaction deleted successfully."));
                NavigationManager.Refresh(true);
            }
            else
            {
                // do nothing
                toastService.Notify(new ToastMessage(ToastType.Secondary, $"Transaction not deleted."));
            }
        }

        //cancel by doing nothing
        private async Task CancelButton()
        {
            await updatemodal.HideAsync();
        }
    }
}