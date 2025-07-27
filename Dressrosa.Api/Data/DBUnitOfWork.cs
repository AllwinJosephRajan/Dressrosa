using System.Data;
using System.Data.Common;

namespace Dressrosa.Api.Data
{
    public class DBUnitOfWork : IDBUnitOfWork
    {
        public DbConnection Connection { get; private set; }

        public DbTransaction Transaction { get; private set; }
        //private readonly string _connectionString;

        private bool _isDisposed;


        public DBUnitOfWork(DbConnection connection)
        {
            Connection = connection;

        }
        public void OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (Connection.State != System.Data.ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        public async Task BeginAsync()
        {
            await Connection.OpenAsync();
            Transaction = await Connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await Transaction.CommitAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                // free managed resources
                Transaction?.Dispose();

                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();
                }
            }
            _isDisposed = true;
        }

        public async Task RollbackAsync()
        {
            await Transaction.RollbackAsync();
        }


    }
}
