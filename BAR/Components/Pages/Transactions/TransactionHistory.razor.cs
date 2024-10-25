using System.Net.Http.Headers;
using BAR.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace BAR.Data.Services
{
    public class TransactionsService
    {
        private ApplicationDbContext dbContext;

        public TransactionsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //SHOW ALL
        public async Task<List<UserTransaction>> getTHistAsync()
        {
            return await dbContext.UserTransactions.ToListAsync();
        }
        //CREATE
        public async Task<UserTransaction> AddTransactionAsync(UserTransaction transaction)
        {
            try
            {
                dbContext.UserTransactions.Add(transaction);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return transaction;
        }
        //UPDATE
        public async Task<UserTransaction> UpdateTransactionAsync(UserTransaction transaction)
        {
            try
            {
                var transactoinExist = dbContext.UserTransactions.FirstOrDefault(p => p.TransactionDateTime == transaction.TransactionDateTime);
                if (transactoinExist != null)
                {
                    dbContext.Update(transaction);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return transaction;
        }
        //REMOVE
        public async Task DeleteTransactionAsync(UserTransaction transaction)
        {
            try
            {
                dbContext.Remove(transaction);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}