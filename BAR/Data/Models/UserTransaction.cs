using System.ComponentModel.DataAnnotations;
using BAR.Data.Models;

public class UserTransaction
{
    [Key]
    public int TransactionId { get; set; }
    public DateTime TransactionDateTime { get; set; }
    public string TransactionLabel { get; set; }
    public decimal TransactionAmt { get; set; }
    public string TransasctionCategory { get; set; }

    // FK from IdentityUser
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

}