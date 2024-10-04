using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BAR.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
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
        public string FName { get; set; }
        public string LName { get; set; }
        //settings
        public bool Darkmode { get; set; }
        public char Currency { get; set; }
    }
}
