using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Mapping.Impl;

namespace Catnap.Mapping
{
    public interface IEntityMap
    {
        string BaseSelectSql { get; }
        string TableName { get; }
        Type EntityType { get; }
        object GetId(object entity);
        void SetId(object entity, object id, ISession session);
        bool IsTransient(object entity);
        IDbCommand GetListCommand(IEnumerable<Parameter> parameters, string whereSql, IDbCommandFactory commandFactory);
        IDbCommand GetListAllCommand(IDbCommandFactory commandFactory);
        IDbCommand GetGetCommand(object id, IDbCommandFactory commandFactory);
        IDbCommand GetDeleteCommand(object id, IDbCommandFactory commandFactory);
        IDbCommand GetInsertCommand(object entity, IDbCommandFactory commandFactory);
        IDbCommand GetInsertCommand(object entity, string parentIdColumnName, object parentId, IDbCommandFactory commandFactory);
        IDbCommand GetUpdateCommand(object entity, IDbCommandFactory commandFactory);
        IDbCommand GetUpdateCommand(object entity, string parentIdColumnName, object parentId, IDbCommandFactory commandFactory);
        IDbCommand GetSaveCommand(object entity, string parentIdColumnName, object parentId, IDbCommandFactory commandFactory);
        string GetColumnNameForProperty(MemberExpression memberExpression);
        void Done(IDomainMap map, IDbAdapter dbAdapter);
    }

    public interface IEntityMap<T> : IEntityMap where T : class, new()
    {
        IEnumerable<IPropertyMap<T>> PropertyMaps { get; }
        T BuildFrom(IDictionary<string, object> record, ISession session);
        string GetColumnNameForProperty(Expression<Func<T, object>> property);
    }
}