using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BAR.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Key]
        [ForeignKey("Id")]
        public required string Uid { get; set; }
        //budget
        public decimal Income { get; set; }
        [NotMapped]
        public string IncSpend { get { return @*FIXME*@} }
        public decimal Housing { get; set; }
        [NotMapped]
        public string HouSpend { get { return @*FIXME*@} }
        public decimal BillsUtilities { get; set; }
        [NotMapped]
        public string BillSpend { get { return @*FIXME*@} }
        public decimal GroceryDining { get; set; }
        [NotMapped]
        public string GroSpend { get { return @*FIXME*@} }

        public decimal Transport { get; set; }
        [NotMapped]
        public string TraSpend { get { return @*FIXME*@} }
        public decimal Education { get; set; }
        [NotMapped]
        public string EduSpend { get { return @*FIXME*@} }
        public decimal Debt { get; set; }
        [NotMapped]
        public string DebSpend { get { return @*FIXME*@} }
        public decimal Entertainment { get; set; }
        [NotMapped]
        public string IncSpend { get { return @*FIXME*@} }
        public decimal Shopping { get; set; }
        public decimal Medical { get; set; }
        public decimal Investing { get; set; }
        public decimal Misc { get; set; }
        //profile
        [AllowNull]
        public Byte[] UserImage { get; set; }
        [AllowNull]
        public string FName { get; set; }
        [AllowNull]
        public string LName { get; set; }
        //settings
        public bool Darkmode { get; set; }
        public char Locale { get; set; }
    }
}