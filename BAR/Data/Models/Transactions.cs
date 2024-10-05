using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BAR.Data.Models
{
    public class Transaction : IdentityUser
    {
        [Key]
        public required byte[] TimeStamp { get; set; }
        [ForeignKey("Id")]
        public required string Uid { get; set; }
        public required string Label { get; set; }
        public required string Category { get; set; }
        public required string Amount { get; set; }
    }
}
