namespace BAR.Data.Interfaces
{
    public interface ITransaction
    {
        public List<UserTransaction> GetTransactions();
        public void GetTransaction(int tid);
        public void AddTransaction(UserTransaction transaction);
        public void UpdateTransaction(UserTransaction transaction);
        public void DeleteTransaction(int tid);
    }
}
