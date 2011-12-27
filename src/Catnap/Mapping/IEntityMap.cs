using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Database;

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
        DbCommandSpec GetFindCommand(IList<Parameter> parameters, IList<string> condtions);
        DbCommandSpec GetGetCommand(object id);
        DbCommandSpec GetDeleteCommand(object id);
        DbCommandSpec GetInsertCommand(object entity);
        DbCommandSpec GetInsertCommand(object entity, string parentIdColumnName, object parentId);
        DbCommandSpec GetUpdateCommand(object entity);
        DbCommandSpec GetUpdateCommand(object entity, string parentIdColumnName, object parentId);
        DbCommandSpec GetSaveCommand(object entity, string parentIdColumnName, object parentId);
        string GetColumnNameForProperty(MemberExpression memberExpression);
        void Done(IDomainMap map, IDbAdapter dbAdapter);
    }

    public interface IEntityMap<T> : IEntityMap where T : class, new()
    {
        IList<IPropertyMap<T>> PropertyMaps { get; }
        T BuildFrom(IDictionary<string, object> record, ISession session);
        string GetColumnNameForProperty(Expression<Func<T, object>> property);
    }
}