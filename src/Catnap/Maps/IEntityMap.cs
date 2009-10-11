using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Common.Database;

namespace Catnap.Maps
{
    public interface IEntityMap
    {
        string BaseSelectSql { get; }
        string TableName { get; }
        string ParentColumnName { get; }
        Type EntityType { get; }
        DbCommandSpec GetFindCommand(IList<Parameter> parameters, IList<string> condtions);
        DbCommandSpec GetGetCommand(int id);
        DbCommandSpec GetDeleteCommand(int id);
        DbCommandSpec GetInsertCommand(IEntity entity);
        DbCommandSpec GetInsertCommand(IEntity entity, int? parentId);
        DbCommandSpec GetUpdateCommand(IEntity entity);
        DbCommandSpec GetUpdateCommand(IEntity entity, int? parentId);
        string GetColumnNameForProperty(MemberExpression memberExpression);
    }

    public interface IEntityMap<T> : IEntityMap where T : class, IEntity, new()
    {
        IList<IPropertyMap<T>> PropertyMaps { get; }
        T BuildFrom(IDictionary<string, object> record, ISession session);
        string GetColumnNameForProperty(Expression<Func<T, object>> property);
    }
}