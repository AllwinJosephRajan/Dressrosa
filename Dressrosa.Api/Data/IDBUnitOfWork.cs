using System.Data.Common;

namespace Dressrosa.Api.Data
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
