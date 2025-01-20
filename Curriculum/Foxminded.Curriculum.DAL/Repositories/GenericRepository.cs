using Dapper;
using Foxminded.Curriculum.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;

namespace Foxminded.Curriculum.DAL.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public GenericRepository(IDbTransaction dbTransaction)
    {
        _dbConnection = dbTransaction.Connection ?? default!;
        _dbTransaction = dbTransaction;
    }

    public async Task<TEntity> GetByIdAsync(int Id)
    {
        string tableName = GetTableName();
        string keyColumn = GetKeyColumnName();
        string query = $"SELECT * FROM {tableName} WHERE {keyColumn} = @Value";

        var parameters = new { Value = Id };
        var result = await _dbTransaction.Connection!.QueryAsync<TEntity>(query, parameters, _dbTransaction);

        return result.FirstOrDefault()!;
    }

    public async Task<IEnumerable<TEntity>> GetByConditionAsync(string columnName, object value)
    {
        string tableName = GetTableName();
        string query = $"SELECT * FROM {tableName} WHERE {columnName} = @Value";

        var parameters = new { Value = value };
        return await _dbTransaction.Connection!.QueryAsync<TEntity>(query, parameters, _dbTransaction);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        string tableName = GetTableName();
        string query = $"SELECT * FROM {tableName}";

        return await _dbTransaction.Connection!.QueryAsync<TEntity>(query, null, _dbTransaction);
    }

    public async Task<bool> AddAsync(TEntity entity)
    {
        string tableName = GetTableName();

        var properties = GetProperties(excludeKey: true)
            .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null)
            .Select(prop => new
            {
                ColumnName = prop.GetCustomAttribute<ColumnAttribute>()!.Name,
                PropertyName = prop.Name,
                Value = prop.GetValue(entity) ?? DBNull.Value
            })
            .ToList();

        string columns = string.Join(", ", properties.Select(p => p.ColumnName));
        string values = string.Join(", ", properties.Select(p => $"@{p.PropertyName}"));

        var parameters = properties.ToDictionary(p => p.PropertyName, p => p.Value);

        string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        var rowsEffected = await _dbTransaction.Connection!.ExecuteAsync(query, parameters, _dbTransaction);

        return rowsEffected > 0 ? true : false;
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        string tableName = GetTableName();
        string keyColumn = GetKeyColumnName();

        if (string.IsNullOrEmpty(keyColumn))
            throw new InvalidOperationException();

        StringBuilder query = new StringBuilder();
        query.Append($"UPDATE {tableName} SET ");

        var parameters = new Dictionary<string, object>();

        foreach (var property in GetProperties(excludeKey: true))
        {
            var columnAttr = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttr == null)
                continue;

            string columnName = columnAttr.Name!;
            string propertyName = property.Name;

            query.Append($"{columnName} = @{propertyName}, ");
            parameters.Add(propertyName, property.GetValue(entity)!);
        }

        query.Remove(query.Length - 2, 2);

        query.Append($" WHERE {keyColumn} = @Id");
        parameters.Add("Id", entity.Id);

        var rowsEffected = await _dbTransaction.Connection!.ExecuteAsync(query.ToString(), parameters, _dbTransaction);

        return rowsEffected > 0 ? true : false;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        string tableName = GetTableName();
        string keyColumn = GetKeyColumnName();
        string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @Value";

        var parameters = new { Value = entity.Id };

        var rowsEffected = await _dbTransaction.Connection!.ExecuteAsync(query, parameters, _dbTransaction);

        return rowsEffected > 0 ? true : false;
    }

    public async Task<bool> IsDuplicateAsync(TEntity entity)
    {
        string tableName = GetTableName();

        var properties = GetProperties(excludeKey: true)
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() != null)
            .Select(p => new
            {
                ColumnName = p.GetCustomAttribute<ColumnAttribute>()!.Name,
                PropertyName = p.Name,
                Value = p.GetValue(entity) ?? DBNull.Value
            })
            .ToList();

        var conditions = string.Join(" AND ", properties.Select(p => $"{p.ColumnName} = @{p.PropertyName}"));
        var parameters = properties.ToDictionary(p => p.PropertyName, p => p.Value);

        string keyColumn = GetKeyColumnName();
        if (!string.IsNullOrEmpty(keyColumn))
        {
            conditions += $" AND {keyColumn} != @Id";
            parameters.Add("Id", entity.Id);
        }

        string query = $"SELECT COUNT(1) FROM {tableName} WHERE {conditions}";

        var count = await _dbTransaction.Connection!.ExecuteScalarAsync<int>(query, parameters, _dbTransaction);

        return count > 0;
    }

    private string GetTableName()
    {
        var type = typeof(TEntity);

        var tableAttr = type.GetCustomAttribute<TableAttribute>();
        if (tableAttr == null)
            throw new InvalidDataException();

        return tableAttr.Name;
    }

    public static string GetKeyColumnName()
    {
        PropertyInfo[] properties = typeof(TEntity).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

            if (keyAttributes != null && keyAttributes.Length > 0)
            {
                object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

                if (columnAttributes != null && columnAttributes.Length > 0)
                {
                    ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                    return columnAttribute.Name!;
                }
                else
                    return property.Name;
            }
        }

        return null!;
    }


    private string GetColumns(bool excludeKey = false)
    {
        var type = typeof(TEntity);
        var columns = string.Join(", ", type.GetProperties()
            .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
            .Select(p =>
            {
                var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                return columnAttr != null ? columnAttr.Name : p.Name;
            }));

        return columns;
    }

    protected string GetPropertyNames(bool excludeKey = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

        var values = string.Join(", ", properties.Select(p =>
        {
            return $"@{p.Name}";
        }));

        return values;
    }

    protected IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

        return properties;
    }

    protected string GetKeyPropertyName()
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.GetCustomAttribute<KeyAttribute>() != null);

        if (properties.Any())
            return properties.FirstOrDefault()!.Name;

        return null!;
    }
}
