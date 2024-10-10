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
        //id
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string GUID { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }

        //profile
        public byte[] UserImage { get; set; } //Placeholder - color choice
        public required string FName { get; set; }
        public required string LName { get; set; }

        //settings
        public bool Darkmode { get; set; }
        public required string Locale { get; set; }

        //budgeting
        public decimal Income { get; set; }
        public decimal Housing { get; set; }
        public decimal BillsUtilities { get; set; }
        public decimal GroceryDining { get; set; }
        public decimal Transport { get; set; }
        public decimal Education { get; set; }
        public decimal Debt { get; set; }
        public decimal Entertainment { get; set; }
        public decimal Shopping { get; set; }
        public decimal Investing { get; set; }
        public decimal Misc { get; set; }
        public decimal Medical { get; set; }

        //spending (last 30 days or this month?) FIXME
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
        [NotMapped]
        public decimal HouSpend { get; set; } //FIXME

        [NotMapped]
        public decimal BillSpend { get; set; } //FIXME
       
        [NotMapped]
        public decimal GroSpend { get; set; } //FIXME
       
        [NotMapped]
        public decimal TraSpend { get; set; } //FIXME
       
        [NotMapped]
        public decimal EduSpend { get; set; } //FIXME
        
        [NotMapped]
        public decimal DebSpend { get; set; } //FIXME
        
        [NotMapped]
        public decimal EntSpend { get; set; } //FIXME
        
        [NotMapped]
        public decimal ShoSpend { get; set; } //FIXME
        
        [NotMapped]
        public decimal MedSpend { get; set; } //FIXME
        
        [NotMapped]
        public decimal InvSpend { get; set; } //FIXME
        
        [NotMapped]
        public decimal MisSpend { get; set; } //FIXME
    }
}

