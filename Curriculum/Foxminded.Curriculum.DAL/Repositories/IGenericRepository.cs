using Foxminded.Curriculum.Domain.Entities;

namespace Foxminded.Curriculum.DAL.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetByConditionAsync(string columnName, object value);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> AddAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> IsDuplicateAsync(TEntity entity);
}
