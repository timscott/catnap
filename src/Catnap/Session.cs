using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Common.Database;
using Catnap.Common.Logging;
using Catnap.Maps;

namespace Catnap
{
    public class Session : ISession
    {
        private IDbConnection connection;
        private readonly IDomainMap domainMap;

        public Session(IDbConnection connection) : this(connection, Domain.Map) { }

        public Session(IDbConnection connection, IDomainMap domainMap)
        {
            this.connection = connection;
            this.domainMap = domainMap;
        }

        public void Open()
        {
            connection.Open();
            connection.BeginTransaction();
        }

        public IDbCommand CreateCommand(DbCommandSpec commandSpec)
        {
            return connection.CreateCommand(commandSpec);
        }

        public IList<IDictionary<string, object>> List(DbCommandSpec commandSpec)
        {
            return ExecuteQuery(commandSpec).ToList();
        }

        public IList<T> List<T>(DbCommandSpec commandSpec) where T : class, IEntity, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            return List(commandSpec).Select(x => BuildFrom(entityMap, x)).ToList();
        }

        public T Get<T>(int id) where T : class, IEntity, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            return List(entityMap.GetGetCommand(id)).Select(x => BuildFrom(entityMap, x)).FirstOrDefault();
        }

        public void SaveOrUpdate<T>(T entity) where T : class, IEntity, new()
        {
            SaveOrUpdate(entity, null);
        }

        public void SaveOrUpdate<T>(T entity, int? parentId) where T : class, IEntity, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            if (entity.IsTransient)
            {
                ExecuteNonQuery(entityMap.GetInsertCommand(entity, parentId));
                entity.SetId(connection.GetLastInsertId());
            }
            else
            {
                ExecuteNonQuery(entityMap.GetUpdateCommand(entity, parentId));
            }
            Cascade(entityMap, entity);
        }

        private void Cascade<T>(IEntityMap<T> entityMap, T entity) where T : class, IEntity, new()
        {
            var listMaps = entityMap.PropertyMaps.Where(x => x is IListPropertyMap<T>).Cast<IListPropertyMap<T>>();
            foreach (var map in listMaps)
            {
                map.Cascade(this, entity);
            }
        }

        public void Delete<T>(int id) where T : class, IEntity, new()
        {
            var map = domainMap.GetMapFor<T>();
            ExecuteNonQuery(map.GetDeleteCommand(id));
        }

        public void ExecuteNonQuery(DbCommandSpec commandSpec)
        {
            try
            {
                CreateCommand(commandSpec).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<IDictionary<string, object>> ExecuteQuery(DbCommandSpec commandSpec)
        {
            try
            {
                return CreateCommand(commandSpec).ExecuteQuery();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                Log.Error(ex);
                throw;
            }
        }

        public T ExecuteScalar<T>(DbCommandSpec commandSpec)
        {
            try
            {
                return CreateCommand(commandSpec).ExecuteScalar<T>();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                Log.Error(ex);
                throw;
            }
        }

        public void RollbackTransaction()
        {
            connection.RollbackTransaction();
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public T BuildFrom<T>(IEntityMap<T> entityMap, IDictionary<string, object> record) 
            where T : class, IEntity, new()
        {
            return entityMap.BuildFrom(record, this);
        }
    }
}