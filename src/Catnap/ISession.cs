using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Citeria.Conditions;
using Catnap.Mapping;

namespace Catnap
{
    public interface ISession : IDisposable
    {
        void Open();
        IList<IDictionary<string, object>> List(IDbCommandSpec commandSpec);
        IList<T> List<T>(IDbCommandSpec commandSpec) where T : class, new();
        IList<T> List<T>(ICriteria<T> criteria) where T : class, new();
        IList<T> List<T>(Expression<Func<T, bool>> predicate) where T : class, new();
        IList<T> List<T>() where T : class, new();
        T Get<T>(object id) where T : class, new();
        void SaveOrUpdate<T>(T entity) where T : class, new();
        void SaveOrUpdate<T>(T entity, string parentIdColumnName, object parentId) where T : class, new();
        void Delete<T>(object id) where T : class, new();
        void ExecuteNonQuery(IDbCommandSpec commandSpec);
        T ExecuteScalar<T>(IDbCommandSpec commandSpec);
        object ExecuteScalar(IDbCommandSpec commandSpec);
        void RollbackTransaction();
        object ConvertFromDbType(object value, Type type);
        IList<IDictionary<string, object>> GetTableMetaData(string tableName);
        string FormatParameterName(string name);
        IEntityMap<T> GetEntityMapFor<T>() where T : class, new();
    }
}