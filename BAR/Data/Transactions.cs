using System.ComponentModel.DataAnnotations;

namespace BAR.Data
{
    public class Transactions
    {
        [Key]
        public required int Id { get; set; }
        public required string Category { get; set; }
        public required string Amount { get; set; }
        public required string Date { get; set; }
    }
}
