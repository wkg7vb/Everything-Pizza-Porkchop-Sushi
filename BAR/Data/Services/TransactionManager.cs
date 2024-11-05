using BAR.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BAR.Data.Services
{
    public class TransactionManager : ITransaction
    {
        private readonly ApplicationDbContext _dbContext;
        public TransactionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserTransaction>> GetTransactions(string UID)
        {
            return await _dbContext.UserTransactions.Where(u => u.UserId == UID).ToListAsync();
        }

        public async Task<UserTransaction> GetTransaction(int tid)
        {
            var transaction = await _dbContext.UserTransactions.FindAsync(tid);
            if (transaction == null)
            {
                throw new Exception("Transaction Not Found");
            }
            return transaction;
        }

        public async Task AddTransaction(UserTransaction transaction)
        {
            _dbContext.UserTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTransaction(UserTransaction transaction)
        {
            _dbContext.Entry(transaction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTransaction(UserTransaction transaction)
        {
            if(transaction != null)
            {
                _dbContext.UserTransactions.Remove(transaction);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
