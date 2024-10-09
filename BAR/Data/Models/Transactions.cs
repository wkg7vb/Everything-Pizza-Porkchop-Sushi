using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAR.Data.Models
{
    public class Transaction : ApplicationUser
    {
        [Key]
        public required string TimeStamp { get; set; }
        [ForeignKey("Id")]
        public required string Uid { get; set; }
        //user text input
        public required string Label { get; set; }
       
        [AllowedValues("Housing",
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
        public required decimal Amount { get; set; }
    }
}
