using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using System.Data;

namespace Foxminded.Curriculum.DAL;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
}
