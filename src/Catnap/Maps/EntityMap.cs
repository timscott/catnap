using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Common;

namespace Catnap.Maps
{
    public class EntityMap<T> : IEntityMap<T> where T : class, IEntity, new()
    {
        private IList<IPropertyMap<T>> propertyMaps = new List<IPropertyMap<T>>();
        public EntityMap() : this(null) { }

        public EntityMap(string parentColumnName) : this(typeof(T).Name, parentColumnName) { }

        public EntityMap(string tableName, string parentColumnName)
        {
            TableName = tableName;
            EntityType = typeof(T);
            ParentColumnName = parentColumnName;
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

        public EntityMap<T> Property<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var map = new ValuePropertyMap<T, TProperty>(property, null);
            propertyMaps.Add(map);
            return this;
        }

        public EntityMap<T> Property<TProperty>(Expression<Func<T, TProperty>> property, string columnName)
        {
            var map = new ValuePropertyMap<T, TProperty>(property, columnName);
            propertyMaps.Add(map);
            return this;
        }

        public EntityMap<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property)
            where TListMember : class, IEntity, new()
        {
            return List(property, true);
        }

        public EntityMap<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy)
            where TListMember : class, IEntity, new()
        {
            return List(property, isLazy, true, true);
        }

        public EntityMap<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes)
            where TListMember : class, IEntity, new()
        {
            return List(property, isLazy, cascadeSaves, cascaseDeletes, null);
        }

        public EntityMap<T> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascaseDeletes, Expression<Func<TListMember, bool>> filter)
            where TListMember : class, IEntity, new()
        {
            var map = new ListPropertyMap<T, TListMember>(property, isLazy, cascadeSaves, cascaseDeletes, filter);
            propertyMaps.Add(map);
            return this;
        }

        public EntityMap<T> BelongsTo<TPropertyType>(Expression<Func<T, TPropertyType>> property, string columnName) 
            where TPropertyType : class, IEntity, new()
        {
            var map = new BelongsToPropertyMap<T, TPropertyType>(property, columnName);
            propertyMaps.Add(map);
            return this;
        }

        public T BuildFrom(IDictionary<string, object> record, ISession session)
        {
            var instance = Activator.CreateInstance<T>();
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
                command.AddParameter(parameter.Name, parameter.Value);
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
                .SetCommandText(string.Format("{0} where Id = @Id", BaseSelectSql))
                .AddParameter("@Id", id);
        }

        public DbCommandSpec GetDeleteCommand(int id)
        {
            return new DbCommandSpec()
                .SetCommandText(string.Format("delete from {0} where Id = @Id", TableName))
                .AddParameter("@Id", id);
        }

        public DbCommandSpec GetInsertCommand(IEntity entity)
        {
            return GetInsertCommand(entity, null);
        }

        public DbCommandSpec GetInsertCommand(IEntity entity, int? parentId)
        {
            var command = new DbCommandSpec();

            var columnProperties = propertyMaps.Where(x => x is IPropertyMapWithColumn<T>).Cast<IPropertyMapWithColumn<T>>();
            var writableColumns = columnProperties.Where(x => !x.SetterIsPrivate);
            var columnNames = writableColumns.Select(x => x.ColumnName).ToList();
            var paramterNames = writableColumns.Select(x => "@" + x.ColumnName).ToList();

            if (!string.IsNullOrEmpty(ParentColumnName))
            {
                columnNames.Add(ParentColumnName);
                paramterNames.Add("@" + ParentColumnName);
                command.AddParameter("@" + ParentColumnName, parentId);
            }

            var sql = string.Format("insert into {0} ({1}) values ({2})", 
                                    TableName,
                                    string.Join(",", columnNames.ToArray()),
                                    string.Join(",", paramterNames.ToArray()));

            command.SetCommandText(sql);

            foreach (var map in writableColumns)
            {
                command.AddParameter("@" + map.ColumnName, map.GetColumnValue(entity));
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

            var columnProperties = propertyMaps.Where(x => x is IPropertyMapWithColumn<T>).Cast<IPropertyMapWithColumn<T>>();
            var setPairs = columnProperties.Where(x => x.ColumnName != "Id")
                .Select(x => string.Format("{0}=@{0}", x.ColumnName)).ToList();
            if (!string.IsNullOrEmpty(ParentColumnName))
            {
                setPairs.Add(string.Format("{0}=@{0}", ParentColumnName));
                command.AddParameter("@" + ParentColumnName, parentId);
            }

            var sql = string.Format("update {0} set {1} where Id = @Id",
                                    TableName,
                                    string.Join(",", setPairs.ToArray()));

            command.SetCommandText(sql);

            foreach (var map in columnProperties)
            {
                command.AddParameter("@" + map.ColumnName, map.GetColumnValue(entity));
            }

            return command;
        }
    }
}