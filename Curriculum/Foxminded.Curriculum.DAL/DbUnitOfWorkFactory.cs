using Microsoft.Data.SqlClient;

namespace Foxminded.Curriculum.DAL;

public class DbUnitOfWorkFactory : IDbUnitOfWorkFactory
{
    private readonly string _connectionString;

    public DbUnitOfWorkFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        var connection = new SqlConnection(_connectionString);
        return new UnitOfWork(connection);
    }
}
