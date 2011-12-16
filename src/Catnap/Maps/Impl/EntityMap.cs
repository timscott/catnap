using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Common.Logging;
using Catnap.Database;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
    public class EntityMap<T> : IEntityMap<T>, IEntityMappable<T> where T : class, new()
    {
        private readonly IList<IPropertyMap<T>> propertyMaps = new List<IPropertyMap<T>>();
        private object transientIdValue;
        private IIdPropertyMap<T> idProperty;
        private string idColumnName;

        public EntityMap()
        {
            EntityType = typeof(T);
            TableName = typeof (T).Name;
        }

        public string TableName { get; private set; }
        public string ParentColumnName { get; private set; }
        public Type EntityType { get; private set; }

        public object GetId(object entity)
        {
            return idProperty.GetValue((T)entity);
        }

        public void SetId(object entity, object id, ISession session)
        {
            idProperty.SetValue((T)entity, id, session);
        }

        public bool IsTransient(object entity)
        {
            var id = GetId(entity);
            return Equals(id, transientIdValue);
        }

        public IList<IPropertyMap<T>> PropertyMaps
        {
            get { return propertyMaps; }
        }

        public string GetColumnNameForProperty(MemberExpression memberExpression)
        {
            var map = propertyMaps.Where(x => x is IPropertyMapWithColumn<T> &&  x.PropertyInfo == memberExpression.Member)
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

        public IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var map = new IdPropertyMap<T, TProperty>(property);
            return Map(map);
        }

        public IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property, Access access)
        {
            var map = new IdPropertyMap<T, TProperty>(property, access);
            return Map(map);
        }

        public IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property, Access access, Generator generator)
        {
            var map = new IdPropertyMap<T, TProperty>(property, access, generator);
            return Map(map);
        }

        public IEntityMappable<T> Id<TProperty>(Expression<Func<T, TProperty>> property, string columnName, Access access, Generator generator)
        {
            var map = new IdPropertyMap<T, TProperty>(property, columnName, access, generator);
            return Map(map);
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
            where TListMember : class, new()
        {
            return List(property, true);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy)
            where TListMember : class, new()
        {
            return List(property, isLazy, true, true);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes)
            where TListMember : class, new()
        {
            return List(property, isLazy, cascadeSaves, cascaseDeletes, null);
        }

        public IEntityMappable<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes, Expression<Func<TListMember, bool>> filter)
            where TListMember : class, new()
        {
            var map = new ListPropertyMap<T, TListMember>(property, isLazy, cascadeSaves, cascaseDeletes, filter);
            return Map(map);
        }

        public IEntityMappable<T> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property)
            where TProperty : class, new()
        {
            var map = new BelongsToPropertyMap<T, TProperty>(property);
            return Map(map);
        }

        public IEntityMappable<T> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property, string columnName) 
            where TProperty : class, new()
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
            var listMaps = propertyMaps.Where(x => x is IListPropertyMap);
            foreach (IListPropertyMap map in listMaps)
            { 
                map.SetMaps(this, domainMap.GetMapFor(map.ItemType));
            }
            var belongsToMaps = propertyMaps.Where(x => x is IBelongsToPropertyMap);
            foreach (IBelongsToPropertyMap map in belongsToMaps)
            {
                map.SetPropertyMap(domainMap.GetMapFor(map.PropertyType));
            }
            var idProperties = propertyMaps.Where(x => x is IIdPropertyMap<T>).ToList();
            if (idProperties.Any() == false)
            {
                var map = new IdPropertyMap<T, object>("Id", "Id", Access.CamelCaseField);
                Map(map);
            } 
            else if (idProperties.Count() > 1)
            {
                throw new Exception(string.Format("Id property was mapped more {0} times for {1} entity. It is not valid to map more than one property as the Id.", idProperties.Count(), typeof(T).Name));
            }
            idProperty = propertyMaps.Where(x => x is IIdPropertyMap<T>).Cast<IIdPropertyMap<T>>().Single();
            idColumnName = idProperty.ColumnName;
            transientIdValue = Activator.CreateInstance(idProperty.PropertyInfo.PropertyType);
        }

        public T BuildFrom(IDictionary<string, object> record, ISession session)
        {
            var instance = Activator.CreateInstance<T>();
            var id = record[idColumnName];
            SetId(instance, id, session);
            foreach (var map in propertyMaps)
            {
                if (map is IPropertyMapWithColumn<T>)
                {
                    map.SetValue(instance, record[((IPropertyMapWithColumn<T>)map).ColumnName], session);
                }
                else if (map is IListPropertyMap<T>)
                {
                    map.SetValue(instance, record[idColumnName], session);
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

        public DbCommandSpec GetGetCommand(object id)
        {
            return new DbCommandSpec()
                .SetCommandText(string.Format("{0} where Id = {1}Id", BaseSelectSql,
                    SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX))
                .AddParameter("Id", id);
        }

        public DbCommandSpec GetDeleteCommand(object id)
        {
            return new DbCommandSpec()
                .SetCommandText(string.Format("delete from {0} where Id = {1}Id", TableName,
                    SessionFactory.DEFAULT_SQL_PARAMETER_PREFIX))
                .AddParameter("Id", id);
        }

        public DbCommandSpec GetInsertCommand(object entity)
        {
            return GetInsertCommand(entity, null);
        }

        public DbCommandSpec GetInsertCommand(object entity, object parentId)
        {
            var command = new DbCommandSpec();

            var columnProperties = propertyMaps.Where(x => x is IPropertyMapWithColumn<T>).Cast<IPropertyMapWithColumn<T>>();
            var writableColumns = columnProperties.Where(x => x.Insert).ToList();
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
                var value = map.GetValue((T)entity);
                var idMap = map as IIdPropertyMap<T>;
                if (idMap != null && Equals(value, transientIdValue))
                {
                    value = idMap.Generate((T)entity);
                }
                command.AddParameter(map.ColumnName, value);
            }

            return command;
        }

        public DbCommandSpec GetUpdateCommand(object entity)
        {
            return GetUpdateCommand(entity, null);
        }

        public DbCommandSpec GetUpdateCommand(object entity, object parentId)
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
            command.AddParameter("Id", GetId(entity));

            command.SetCommandText(sql);

            foreach (var map in columnProperties)
            {
                command.AddParameter(map.ColumnName, map.GetValue((T)entity));
            }

            return command;
        }
    }
}