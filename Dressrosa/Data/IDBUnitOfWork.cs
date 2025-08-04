using System.Data.Common;

namespace Dressrosa.Data
{
    public interface IDBUnitOfWork : IDisposable
    {
        DbConnection Connection { get; }

        DbTransaction Transaction { get; }

        Task BeginAsync();

        Task CommitAsync();
        Task RollbackAsync();
    }
}
