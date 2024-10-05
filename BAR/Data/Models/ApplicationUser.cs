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
        //budget
        public decimal Income { get; set; }
        [NotMapped]
        public decimal TotalSpend
        {
            get
            {
                return HouSpend + BillSpend + GroSpend + TraSpend +
                    EduSpend + DebSpend + EntSpend + ShoSpend + MedSpend +
                    InvSpend + MisSpend;
            }
        }
        public decimal Housing { get; set; }
        [NotMapped]
        public decimal HouSpend { get; set; } //FIXME
        public decimal BillsUtilities { get; set; }
        [NotMapped]
        public decimal BillSpend { get; set; } //FIXME
        public decimal GroceryDining { get; set; }
        [NotMapped]
        public decimal GroSpend { get; set; } //FIXME

        public decimal Transport { get; set; }
        [NotMapped]
        public decimal TraSpend { get; set; } //FIXME
        public decimal Education { get; set; }
        [NotMapped]
        public decimal EduSpend { get; set; } //FIXME
        public decimal Debt { get; set; }
        [NotMapped]
        public decimal DebSpend { get; set; } //FIXME
        public decimal Entertainment { get; set; }
        [NotMapped]
        public decimal EntSpend { get; set; } //FIXME
        public decimal Shopping { get; set; }
        [NotMapped]
        public decimal ShoSpend { get; set; } //FIXME
        public decimal Medical { get; set; }
        [NotMapped]
        public decimal MedSpend { get; set; } //FIXME
        public decimal Investing { get; set; }
        [NotMapped]
        public decimal InvSpend { get; set; } //FIXME
        public decimal Misc { get; set; }
        [NotMapped]
        public decimal MisSpend { get; set; } //FIXME
        //profile
        [AllowNull]
        public byte[] UserImage { get; set; }
        [AllowNull]
        public string FName { get; set; }
        [AllowNull]
        public string LName { get; set; }
        //settings
        public bool Darkmode { get; set; }
        public char Locale { get; set; }
    }
}

