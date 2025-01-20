using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.Concurrent;
using System.Data;

namespace Foxminded.Curriculum.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _dbConnection;
    private IDbTransaction _dbTransaction;

    private readonly ConcurrentDictionary<Type, object> _repositories;
    private bool _disposed = false;

    public UnitOfWork(IDbConnection dbConnection)
    {
        _repositories = new ConcurrentDictionary<Type, object>();
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _dbConnection.Open();
        _dbTransaction = _dbConnection.BeginTransaction();
    }

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        return _repositories.GetOrAdd(typeof(T), _ => new GenericRepository<T>(_dbTransaction))
            as IGenericRepository<T> ?? default!;
    }

    public async Task CommitAsync() => await CompleteTransaction(commit: true);

    public async Task RollbackAsync() => await CompleteTransaction(commit: false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DisposeTransaction();
            }

            _disposed = true;
        }
    }

    private async Task CompleteTransaction(bool commit)
    {
        try
        {
            if (commit)
                await Task.Run(() => _dbTransaction.Commit());
            else
                await Task.Run(() => _dbTransaction.Rollback());
        }
        finally
        {
            DisposeTransaction();
        }
    }

    private void DisposeTransaction()
    {
        _dbTransaction.Dispose();
        _repositories.Clear();
        _dbConnection?.Close();
        _dbConnection?.Dispose();
    }
}
