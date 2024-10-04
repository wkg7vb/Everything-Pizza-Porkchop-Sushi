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
        public string TotalSpend 
        { get
            {
                return HouSpend + BillSpend + GroSpend + TraSpend +
                    EduSpend + DebSpend + EntSpend + ShoSpend + MedSpend +
                    InvSpend + MisSpend;
            } 
        }
        public decimal Housing { get; set; }
        [NotMapped]
        public string HouSpend 
        { @*UNDER CONSTRUCTION *@
            get 
            {
                var spend = from s in Transactions
                            where s.Uid = Uid
                            select s;
                    return spend.ToString();
            } 
        }
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
        public string EntSpend { get { return @*FIXME*@} }
        public decimal Shopping { get; set; }
        [NotMapped]
        public string ShoSpend { get { return @*FIXME*@} }
        public decimal Medical { get; set; }
        [NotMapped]
        public string MedSpend { get { return @*FIXME*@} }
        public decimal Investing { get; set; }
        [NotMapped]
        public string InvSpend { get { return @*FIXME*@} }
        public decimal Misc { get; set; }
        [NotMapped]
        public string MisSpend { get { return @*FIXME*@} }
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

