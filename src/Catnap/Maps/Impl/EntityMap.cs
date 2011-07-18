using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Adapters;
using Catnap.Common.Logging;
using Catnap.Database;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
    public class EntityMap<T> : IEntityMap<T>, IEntityMappable<T> where T : class, IEntity, new()
    {
        private readonly IList<IPropertyMap<T>> propertyMaps = new List<IPropertyMap<T>>();

        public EntityMap()
        {
            EntityType = typeof(T);
            TableName = typeof (T).Name;
        }

        public string TableName { get; private set; }
        public string ParentColumnName { get; private set; }
        public Type EntityType { get; private set; }
        public IList<IPropertyMap<T>> PropertyMaps
        {
            get { return propertyMaps; }
        }

        public string GetColumnNameForProperty(MemberExpression memberExpression)
        {
            var map = propertyMaps.Where(x => x is IPropertyMapWithColumn<T> &&  x.MemberExpression.Member == memberExpression.Member)
                .Cast<IPropertyMapWithColumn<T>>().FirstOrDefault();
            return map.ColumnName;
        }

        public string GetColumnNameForProperty(Expression<Func<T, object>> property)
        {
            return GetColumnNameForProperty(property.GetMemberExpression());
        }

        public IEntityMappable<T> Map(IPropertyMap<T> propertyMap)
        {
            propertyMaps.Add(propertyMap);
            return this;
        }

        public IEntityMappable<T> Property<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var map = new ValuePropertyMap<T, TProperty>(property, null);
            return Map(map);
        }

        public IEntityMappable<T> Property<TProperty>(Expression<Func<T, TProperty>> property, string columnName)
        {
            var map = new ValuePropertyMap<T, TProperty>(property, columnName);
            return Map(map);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property)
            where TListMember : class, IEntity, new()
        {
            return List(property, true);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy)
            where TListMember : class, IEntity, new()
        {
            return List(property, isLazy, true, true);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes)
            where TListMember : class, IEntity, new()
        {
            return List(property, isLazy, cascadeSaves, cascaseDeletes, null);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes, Expression<Func<TListMember, bool>> filter)
            where TListMember : class, IEntity, new()
        {
            var map = new ListPropertyMap<T, TListMember>(property, isLazy, cascadeSaves, cascaseDeletes, filter);
            return Map(map);
        }

        public IEntityMappable<T> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property)
            where TProperty : class, IEntity, new()
        {
            var map = new BelongsToPropertyMap<T, TProperty>(property);
            return Map(map);
        }

        public IEntityMappable<T> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property, string columnName) 
            where TProperty : class, IEntity, new()
        {
            var map = new BelongsToPropertyMap<T, TProperty>(property, columnName);
            return Map(map);
        }

        public IEntityMappable<T> ParentColumn(string parentColumnName)
        {
            ParentColumnName = parentColumnName;
            return this;
        }

        public IEntityMappable<T> Table(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public void Done(IDomainMap domainMap)
        {
            Log.Debug("Setting list maps for entity '{0}'", EntityType.Name);
            var listMaps = propertyMaps.Where(x => x is IListPropertyMap<T>);
            foreach (IListPropertyMap<T> map in listMaps)
            { 
                map.SetListMap(domainMap.GetMapFor(map.ItemTpye));
            }
        }

        public T BuildFrom(IDictionary<string, object> record, ISession session)
        {
            var instance = Activator.CreateInstance<T>();
            var id = Convert.ToInt32(record["Id"]);
            instance.SetId(id);
            foreach (var map in propertyMaps)
            {
                if (map is IPropertyMapWithColumn<T>)
                {
                    map.SetValue(instance, record[((IPropertyMapWithColumn<T>)map).ColumnName], session);
                }
                else if (map is IListPropertyMap<T>)
                {
                    map.SetValue(instance, record["Id"], session);
                }
            }
            return instance;
        }

        public string BaseSelectSql
        {
            get { return string.Format("select * from {0}", TableName); }
        }

        public DbCommandSpec GetFindCommand(IList<Parameter> parameters, IList<string> condtions)
        {
            var command = new DbCommandSpec();
            foreach (var parameter in parameters)
            {
                command.AddParameters(parameter);
            }
            var sql = BaseSelectSql;
            if (condtions.Count > 0)
            {
                sql += " where " + string.Join(" and ", condtions.ToArray());
            }
            command.SetCommandText(sql);
            return command;
        }

        public DbCommandSpec GetGetCommand(int id)
        {
            return new DbCommandSpec()
                .SetCommandText(string.Format("{0} where Id = {1}Id", BaseSelectSql,
                    SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX))
                .AddParameter("Id", id);
        }

        public DbCommandSpec GetDeleteCommand(int id)
        {
            return new DbCommandSpec()
                .SetCommandText(string.Format("delete from {0} where Id = {1}Id", TableName,
                    SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX))
                .AddParameter("Id", id);
        }

        public DbCommandSpec GetInsertCommand(IEntity entity)
        {
            return GetInsertCommand(entity, null);
        }

        public DbCommandSpec GetInsertCommand(IEntity entity, int? parentId)
        {
            var command = new DbCommandSpec();

            var columnProperties = propertyMaps.Where(x => x is IPropertyMapWithColumn<T>).Cast<IPropertyMapWithColumn<T>>();
            var writableColumns = columnProperties.Where(x => !x.SetterIsPrivate).ToList();
            var columnNames = writableColumns.Select(x => x.ColumnName).ToList();
            var paramterNames = writableColumns.Select(x => x.ColumnName).ToList();

            if (!string.IsNullOrEmpty(ParentColumnName))
            {
                columnNames.Add(ParentColumnName);
                paramterNames.Add(ParentColumnName);
                command.AddParameter(ParentColumnName, parentId);
            }

            var sql = string.Format("insert into {0} ({1}) values ({2})", 
                TableName,
                string.Join(",", columnNames.ToArray()),
                string.Join(",", paramterNames.Select(x => SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX + x).ToArray()));

            command.SetCommandText(sql);

            foreach (var map in writableColumns)
            {
                command.AddParameter(map.ColumnName, map.GetColumnValue(entity));
            }

            return command;
        }

        public DbCommandSpec GetUpdateCommand(IEntity entity)
        {
            return GetUpdateCommand(entity, null);
        }

        public DbCommandSpec GetUpdateCommand(IEntity entity, int? parentId)
        {
            var command = new DbCommandSpec();

            var columnProperties = propertyMaps
                .Where(x => x is IPropertyMapWithColumn<T>)
                .Cast<IPropertyMapWithColumn<T>>()
                .ToList();
            var setPairs = columnProperties
                .Where(x => x.ColumnName != "Id")
                .Select(x => string.Format("{0}={1}{0}", x.ColumnName, SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX))
                .ToList();
            if (!string.IsNullOrEmpty(ParentColumnName))
            {
                setPairs.Add(string.Format("{0}={1}{0}", ParentColumnName, SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX));
                command.AddParameter(ParentColumnName, parentId);
            }

            var sql = string.Format("update {0} set {1} where Id = {2}Id",
                TableName,
                string.Join(",", setPairs.ToArray()),
                SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX);
            command.AddParameter("Id", entity.Id);

            command.SetCommandText(sql);

            foreach (var map in columnProperties)
            {
                command.AddParameter(map.ColumnName, map.GetColumnValue(entity));
            }

            return command;
        }
    }
}