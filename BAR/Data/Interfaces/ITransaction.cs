namespace BAR.Data.Interfaces
{
    public interface ITransaction
    {
        Task<List<UserTransaction>> GetTransactions();
        Task<UserTransaction> GetTransaction(int tid);
        Task AddTransaction(UserTransaction transaction);
        Task UpdateTransaction(UserTransaction transaction);
        Task DeleteTransaction(int tid);
    }
}
