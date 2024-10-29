using BAR.Data.Interfaces;
using BAR.Data.Models;
using Microsoft.Identity.Client;

namespace BAR.Data.Services
{
    public class TransactionManager : ITransaction
    {
        readonly ApplicationDbContext _dbContext = new();
        public TransactionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<UserTransaction> GetTransactions()
        {
            try
            {
                return _dbContext.UserTransactions.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AddTransaction(UserTransaction transaction)
        {
            try
            {
                _dbContext.UserTransactions.Add(transaction);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public UserTransaction GetTransaction(int tid)
        {
            try
            {
                UserTransaction? transaction = _dbContext.UserTransactions.Find(tid);
                if (_dbContext.UserTransactions != null)
                {
                    return transaction;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateTransaction(UserTransaction transaction)
        {
            try
            {
                _dbContext.Entry(transaction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        public void DeleteTransaction(int tid)
        {
            try
            {
                UserTransaction? transaction = _dbContext.UserTransactions.Find(tid);
                if(transaction != null)
                {
                    _dbContext.UserTransactions.Remove(transaction);
                    _dbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
