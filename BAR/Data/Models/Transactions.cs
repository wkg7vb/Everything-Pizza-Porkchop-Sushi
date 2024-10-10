using BAR.Components.Pages.TransactionHist;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAR.Data.Models
{
    public class Transaction : ApplicationUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public required string TimeStamp { get; set; }

        [ForeignKey("GUID")]
        public required string GUID { get; set; }

        [AllowedValues(
           "Housing",
           "Bills/Utilities",
           "Grocery/Dining",
           "Transportation",
           "Education",
           "Debt",
           "Entertainment",
           "Shopping",
           "Medical",
           "Investing",
           "Miscellaneous")]
        public required string Category { get; set; }

        //user inputs
        public required string Label { get; set; }
        public required decimal Amount { get; set; }

        //constructor
        public Transaction(string guid, TSingle tuple)
        {
            TimeStamp = DateTimeOffset.Now.ToString("yyyyMMddHHmmssffff");
            GUID = guid;
            Category = tuple.Category;
            Label = tuple.Label;
            Amount = tuple.Amount;
        }
    }
}
