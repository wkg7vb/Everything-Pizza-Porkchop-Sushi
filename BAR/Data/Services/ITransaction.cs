namespace BAR.Data.Services
{
    public interface ITransaction
    {
        Task<List<UserTransaction>> GetTransactions(string UID);
        Task<UserTransaction> GetTransaction(int tid);
        Task AddTransaction(UserTransaction transaction);
        Task UpdateTransaction(UserTransaction transaction);
        Task DeleteTransaction(UserTransaction transaction);
    }
}
