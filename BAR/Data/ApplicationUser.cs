using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BAR.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Key]
        //public required int Id { get; set; }

        //auth
        //public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; } //STORE ONLY A HASH OF THIS!

        //budget
        public decimal Income { get; set; }
        public decimal Housing { get; set; }
        public decimal BillsUtilities { get; set; }
        public decimal GroceryDining { get; set; }
        public decimal Transport { get; set; }
        public decimal Education { get; set; }
        public decimal Debt { get; set; }
        public decimal Entertainment { get; set; }
        public decimal Shopping { get; set; }
        public decimal Medical { get; set; }
        public decimal Investing { get; set; }
        public decimal Misc { get; set; }

        //profile
        public required string FName { get; set; }
        public required string LName { get; set; }
        //settings
        public bool Darkmode { get; set; }
        public char Currency { get; set; }
    }
}
