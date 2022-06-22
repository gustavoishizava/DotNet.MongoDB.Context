namespace MeuBolsoDigital.MongoDB.Context.Context.Interfaces
{
    internal interface IDbContextTransactionOperations
    {
        void StartTransaction();
        Task CommitAsync();
        Task RollbackAsync();
    }
}