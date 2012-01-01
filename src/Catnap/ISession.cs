using System;
using System.Collections.Generic;
using Catnap.Citeria.Conditions;
using Catnap.Database;

namespace Catnap
{
    public interface ISession : IDisposable
    {
        void Open();
        IList<IDictionary<string, object>> List(DbCommandSpec commandSpec);
        IList<T> List<T>(DbCommandSpec commandSpec) where T : class, new();
        IList<T> List<T>(ICriteria<T> criteria) where T : class, new();
        IList<T> List<T>() where T : class, new();
        T Get<T>(object id) where T : class, new();
        void SaveOrUpdate<T>(T entity) where T : class, new();
        void SaveOrUpdate<T>(T entity, string parentIdColumnName, object parentId) where T : class, new();
        void Delete<T>(object id) where T : class, new();
        void ExecuteNonQuery(DbCommandSpec commandSpec);
        T ExecuteScalar<T>(DbCommandSpec commandSpec);
        object ExecuteScalar(DbCommandSpec commandSpec);
        void RollbackTransaction();
        object ConvertFromDbType(object value, Type type);
        IList<IDictionary<string, object>> GetTableMetaData(string tableName);
        string FormatParameterName(string name);
        void Build<T>(ICriteria<T> criteria) where T : class, new();
    }
}