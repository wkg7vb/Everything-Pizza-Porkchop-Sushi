using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAR.Data.Models
{
    public class Transaction
    {
        [Key]
        public required string TimeStamp { get; set; }
        [ForeignKey("GUID")]
        public required string GUID { get; set; }
        public required string Label { get; set; }
        public required string Category { get; set; }
        public required decimal Amount { get; set; }
    }
}
