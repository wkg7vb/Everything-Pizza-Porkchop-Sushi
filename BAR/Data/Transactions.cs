using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BAR.Data
{
    public class Transactions : IdentityUser
    {
        public required string Category { get; set; }
        public required string Amount { get; set; }
        public required string Date { get; set; }
    }
}
