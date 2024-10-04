using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        public decimal TotalSpend 
        { get
            {
                return HouSpend + BillSpend + GroSpend + TraSpend +
                    EduSpend + DebSpend + EntSpend + ShoSpend + MedSpend +
                    InvSpend + MisSpend;
            } 
        }
        public decimal Housing { get; set; }
        [NotMapped]
        public decimal HouSpend 
        {//UNDER CONSTRUCTION
            get 
            {
                using (var context = new ApplicationDbContext())
                {
                    var log = from b in context.Transactions
                              where b.Uid == Uid
                              //where TimeStamp == This month
                              select b;
                    decimal total = 0;
                    //sum the transactions in log
                    return total;
                }
            } 
        }
        public decimal BillsUtilities { get; set; }
        [NotMapped]
        public decimal BillSpend { get { return /*FIXME*/} }
        public decimal GroceryDining { get; set; }
        [NotMapped]
        public decimal GroSpend { get { return /*FIXME*/} }

        public decimal Transport { get; set; }
        [NotMapped]
        public decimal TraSpend { get { return /*FIXME*/} }
        public decimal Education { get; set; }
        [NotMapped]
        public decimal EduSpend { get { return /*FIXME*/} }
        public decimal Debt { get; set; }
        [NotMapped]
        public decimal DebSpend { get { return /*FIXME*/} }
        public decimal Entertainment { get; set; }
        [NotMapped]
        public decimal EntSpend { get { return /*FIXME*/} }
        public decimal Shopping { get; set; }
        [NotMapped]
        public decimal ShoSpend { get { return /*FIXME*/} }
        public decimal Medical { get; set; }
        [NotMapped]
        public decimal MedSpend { get { return /*FIXME*/} }
        public decimal Investing { get; set; }
        [NotMapped]
        public decimal InvSpend { get { return /*FIXME*/} }
        public decimal Misc { get; set; }
        [NotMapped]
        public decimal MisSpend { get { return /*FIXME*/} }
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

