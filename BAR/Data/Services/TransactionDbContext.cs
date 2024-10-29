using Microsoft.EntityFrameworkCore;

namespace BAR.Data.Services
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {

        }

        public DbSet<UserTransaction> Transactions { get; set; }
    }
}
