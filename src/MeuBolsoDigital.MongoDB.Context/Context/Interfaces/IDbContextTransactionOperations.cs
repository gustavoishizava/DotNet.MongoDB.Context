namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    public interface IDbContextTransactionOperations
    {
        void StartTransaction();
        Task CommitAsync();
        Task RollbackAsync();
    }
}