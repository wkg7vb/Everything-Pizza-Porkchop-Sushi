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
        public async Task<List<Transaction>> getTHistAsync()
        {
            return await dbContext.Transactions.ToListAsync();
        }
        //CREATE
        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            try
            {
                dbContext.Transactions.Add(transaction);
                await dbContext.SaveChangesAsync();
            }
            catch(Exception)
            {
                throw;
            }
            return transaction;
        }
        //UPDATE
        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            try
            {
                var transactoinExist = dbContext.Transactions.FirstOrDefault(p => p.TimeStamp == transaction.TimeStamp);
                if (transactoinExist != null)
                {
                    dbContext.Update(transaction);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch(Exception)
            {
                throw;
            }
            return transaction;
        }
        //REMOVE
        public async Task DeleteTransactionAsync(Transaction transaction)
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

