using BlazorBootstrap;

namespace BAR.Components.Pages.Settings;

public partial class Settings
{
    private Modal dltacct = default!;
    private Modal avtrclr = default!;
    // Pulled from db, REMOVE DEFAULTS LATER
    private string UserScreenname = "RowdyRootinRoo";
    private string UserFName = "Rootin'";
    private string UserLName = "Roo";
    private string UserAvatarColor = "#E5A000";
    private string UserEmail = "roo@rootin.com";
    private string UserLocale = "es-MX";
    private bool UserDarkMode = false;

    // Used to store inputs; to be used to update db on save
    private string userNameInput;
    private string userEmailInput;
    private bool darkModeInput;
    private string userLocaleInput;
    private string? usernameConfirmationInput;
    private bool disableName = true;
    private bool disableEmail = true;
    private bool disableDelete = false;
    private string selectedColor;
    private UserDetails ud = new UserDetails();

    private List<string> colors = new List<string>
    {
        "#E5A000",
        "#00c234",
        "#04a9d6",
        "#7104d6",
        "#d602c1",
        "#d60202",
        "#ffff00"
    };

    public class UserDetails
    {
    }

    // Functions

    protected override void OnInitialized()
    {
        // Set input values to those pulled from db
        userNameInput = UserFName + " " + UserLName;
        userEmailInput = UserEmail;
        darkModeInput = UserDarkMode;
        userLocaleInput = UserLocale;
        selectedColor = UserAvatarColor;
    }

    private void ToggleEditName()
    {
        if (!disableEmail) return;
        else
        {
            if (disableName)
            {
                disableName = false;
            }
            else disableName = true;
        }
    }
    private void ToggleEditEmail()
    {
        if (!disableName) return;
        else
        {
            if (disableEmail)
            {
                disableEmail = false;
            }
            else disableEmail = true;
        }
    }
    private async Task DeleteAccount()
    {
        await dltacct.ShowAsync();
    }

    private async Task CancelDeleteAccount()
    {
        usernameConfirmationInput = string.Empty;
        await dltacct.HideAsync();
    }

    private async Task ChangeAvatar()
    {
        await avtrclr.ShowAsync();
    }

    private async Task CancelChangeAvatar()
    {
        selectedColor = UserAvatarColor;
        await avtrclr.HideAsync();
    }

    private void SelectColor(string col)
    {
        selectedColor = col;
    }

    // TODO: actually change some value to update new color
    private async Task ChooseChangeAvatar()
    {
        await avtrclr.HideAsync();
    }

    private void DeleteAccountProcedure()
    {
        // TODO: add delete account procedure
    }

    private void SaveChanges()
    {
        // TODO: add saving to db functionality
    }
}