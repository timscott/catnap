using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Database;
using Catnap.Extensions;
using Catnap.Logging;

namespace Catnap.Mapping.Impl
{
    public class EntityMap<T> : IEntityMap<T>, IEntityMappable<T> where T : class, new()
    {
        private readonly IList<IPropertyMap<T>> propertyMaps = new List<IPropertyMap<T>>();
        private IDbAdapter dbAdapter;
        private object transientIdValue;
        private IIdPropertyMap<T> idProperty;
        private string idColumnName;

        public EntityMap(Action<IEntityMappable<T>> propertyMappings)
        {
            EntityType = typeof(T);
            TableName = typeof (T).Name;
            propertyMappings(this);
        }

        public string TableName { get; private set; }
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

        public IEnumerable<IPropertyMap<T>> PropertyMaps
        {
            get { return propertyMaps; }
        }

        public string GetColumnNameForProperty(MemberExpression memberExpression)
        {
            var map = propertyMaps.Where(x => 
                x is IPropertyMapWithColumn<T> && 
                x.PropertyInfo == memberExpression.Member)
                .Cast<IPropertyMapWithColumn<T>>().FirstOrDefault();
            if (map == null)
            {
                throw new InvalidOperationException(string.Format("Property '{0}' is not mapped.", memberExpression.Member.Name));
            }
            return map.GetColumnName();
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

        public IdPropertyMap<T, TProperty> Id<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var map = new IdPropertyMap<T, TProperty>(property);
            propertyMaps.Add(map);
            return map;
        }

        public ValuePropertyMap<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var map = new ValuePropertyMap<T, TProperty>(property);
            propertyMaps.Add(map);
            return map;
        }

        public ListPropertyMap<T, TListMember> List<TListMember>(Expression<Func<T, IEnumerable<TListMember>>> property)
            where TListMember : class, new()
        {
            var map = new ListPropertyMap<T, TListMember>(property);
            propertyMaps.Add(map);
            return map;
        }

        public BelongsToPropertyMap<T, TProperty> BelongsTo<TProperty>(Expression<Func<T, TProperty>> property)
            where TProperty : class, new()
        {
            var map = new BelongsToPropertyMap<T, TProperty>(property);
            propertyMaps.Add(map);
            return map;
        }

        public IEntityMappable<T> Table(string tableName)
        {
            TableName = tableName;
            return this;
        }

        public void Done(IDomainMap domainMap, IDbAdapter dbAdapter)
        {
            Log.Debug("Setting list maps for entity '{0}'", EntityType.Name);
            this.dbAdapter = dbAdapter;
            var listMaps = propertyMaps.Where(x => x is IListPropertyMap);
            foreach (IListPropertyMap map in listMaps)
            { 
                map.Done(domainMap, this, domainMap.GetMapFor(map.ItemType));
            }
            var belongsToMaps = propertyMaps.Where(x => x is IBelongsToPropertyMap);
            foreach (IBelongsToPropertyMap map in belongsToMaps)
            {
                map.Done(domainMap.GetMapFor(map.PropertyType));
            }
            var idProperties = propertyMaps.Where(x => x is IIdPropertyMap<T>).ToList();
            if (idProperties.Any() == false)
            {
                var idMap = domainMap.IdMappingConvention.GetMap<T>(this);
                Map(idMap);
            } 
            else if (idProperties.Count() > 1)
            {
                throw new Exception(string.Format("Id property was mapped {0} times for {1} entity. It is not valid to map more than one property as the Id.", idProperties.Count(), typeof(T).Name));
            }
            foreach (var map in propertyMaps)
            {
                map.Done(domainMap);
            }
            idProperty = propertyMaps.Where(x => x is IIdPropertyMap<T>).Cast<IIdPropertyMap<T>>().Single();
            idColumnName = idProperty.GetColumnName();
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
                    map.SetValue(instance, record[((IPropertyMapWithColumn<T>)map).GetColumnName()], session);
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

        public IDbCommand GetListCommand(IEnumerable<Parameter> parameters, string whereSql, IDbCommandFactory commandFactory)
        {
            return commandFactory.Create(parameters, BaseSelectSql + " where " + whereSql);
        }

        public IDbCommand GetListAllCommand(IDbCommandFactory commandFactory)
        {
            return commandFactory.Create(null, BaseSelectSql);
        }

        public IDbCommand GetGetCommand(object id, IDbCommandFactory commandFactory)
        {
            var sql = string.Format("{0} where {1} = {2}", BaseSelectSql, idColumnName, dbAdapter.FormatParameterName(idColumnName));
            var parameters = new[] { new Parameter(idColumnName, id) };
            return commandFactory.Create(parameters, sql);
        }

        public IDbCommand GetDeleteCommand(object id, IDbCommandFactory commandFactory)
        {
            var sql = string.Format("delete from {0} where {1} = {2}", TableName, idColumnName, dbAdapter.FormatParameterName(idColumnName));
            var parameters = new[] { new Parameter(idColumnName, id) };
            return commandFactory.Create(parameters, sql);
        }

        public IDbCommand GetInsertCommand(object entity, IDbCommandFactory commandFactory)
        {
            return GetInsertCommand(entity, null, null, commandFactory);
        }

        public IDbCommand GetInsertCommand(object entity, string parentIdColumnName, object parentId, IDbCommandFactory commandFactory)
        {
            var columnProperties = propertyMaps.Where(x => x is IPropertyMapWithColumn<T>).Cast<IPropertyMapWithColumn<T>>();
            var writableColumns = columnProperties.Where(x => x.Insert).ToList();
            var columnNames = writableColumns.Select(x => x.GetColumnName()).ToList();
            var paramterNames = writableColumns.Select(x => x.GetColumnName()).ToList();

            var parameters = new List<Parameter>();

            if (!string.IsNullOrEmpty(parentIdColumnName))
            {
                columnNames.Add(parentIdColumnName);
                paramterNames.Add(parentIdColumnName);
                parameters.Add(new Parameter(parentIdColumnName, parentId));
            }

            var sql = string.Format("insert into {0} ({1}) values ({2})", 
                TableName,
                string.Join(",", columnNames.ToArray()),
                string.Join(",", paramterNames.Select(x => dbAdapter.FormatParameterName(x)).ToArray()));

            foreach (var map in writableColumns)
            {
                var value = map.GetValue((T)entity);
                var idMap = map as IIdPropertyMap<T>;
                if (idMap != null && Equals(value, transientIdValue))
                {
                    value = idMap.Generate((T)entity);
                }
                parameters.Add(new Parameter(map.GetColumnName(), value));
            }

            return commandFactory.Create(parameters, sql);
        }

        public IDbCommand GetUpdateCommand(object entity, IDbCommandFactory commandFactory)
        {
            return GetUpdateCommand(entity, null, null, commandFactory);
        }

        public IDbCommand GetUpdateCommand(object entity, string parentIdColumnName, object parentId, IDbCommandFactory commandFactory)
        {

            var columnProperties = propertyMaps
                .Where(x => x is IPropertyMapWithColumn<T>)
                .Cast<IPropertyMapWithColumn<T>>()
                .ToList();
            var setPairs = columnProperties
                .Where(x => x.GetColumnName() != idColumnName)
                .Select(x => string.Format("{0}={1}", x.GetColumnName(), dbAdapter.FormatParameterName(x.GetColumnName())))
                .ToList();

            var parameters = new List<Parameter>();

            if (!string.IsNullOrEmpty(parentIdColumnName))
            {
                setPairs.Add(string.Format("{0}={1}", parentIdColumnName, dbAdapter.FormatParameterName(parentIdColumnName)));
                parameters.Add(new Parameter(parentIdColumnName, parentId));
            }

            var sql = string.Format("update {0} set {1} where {2} = {3}",
                TableName,
                string.Join(",", setPairs.ToArray()),
                idColumnName,
                dbAdapter.FormatParameterName("Id"));
            parameters.Add(new Parameter(idColumnName, GetId(entity)));

            var columnParamters = columnProperties.Select(map => new Parameter(map.GetColumnName(), map.GetValue((T)entity)));
            parameters.AddRange(columnParamters);

            return commandFactory.Create(parameters, sql);
        }

        public IDbCommand GetSaveCommand(object entity, string parentIdColumnName, object parentId, IDbCommandFactory commandFactory)
        {
            return IsTransient(entity) 
                ? GetInsertCommand(entity, parentIdColumnName, parentId, commandFactory) 
                : GetUpdateCommand(entity, parentIdColumnName, parentId, commandFactory);
        }
    }
}