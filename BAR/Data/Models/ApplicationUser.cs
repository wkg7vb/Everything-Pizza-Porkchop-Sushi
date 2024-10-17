using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BAR.Data.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool UserDarkmode { get; set; }
        public string UserLocale { get; set; } = "en-US";
        public string UserAvatarColor { get; set; } = "#E5A000";
        [PersonalData]
        public string? UserFName { get; set; }
        [PersonalData]
        public string? UserLName { get; set; }
    }
}

