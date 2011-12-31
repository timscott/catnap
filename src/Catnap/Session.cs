using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Catnap.Database;
using Catnap.Find.Conditions;
using Catnap.Logging;
using Catnap.Mapping;

namespace Catnap
{
    public class Session : ISession
    {
        private readonly IDbConnection connection;
        private readonly IDomainMap domainMap;
        private readonly IDbAdapter dbAdapter;
        private IDbTransaction transaction;

        public Session(IDomainMap domainMap, string connectionString, IDbAdapter dbAdapter)
        {
            this.domainMap = domainMap;
            this.dbAdapter = dbAdapter;
            connection = dbAdapter.CreateConnection(connectionString);
        }

        public void Open()
        {
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public IList<IDictionary<string, object>> List(DbCommandSpec commandSpec)
        {
            var command = commandSpec.CreateCommand(dbAdapter, connection);
            return Try(() => ExecuteQuery(command)).ToList();
        }

        public IList<T> List<T>(DbCommandSpec commandSpec) where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            return List(commandSpec).Select(x => entityMap.BuildFrom(x, this)).ToList();
        }

        public IList<T> List<T>(ICriteria<T> criteria) where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            criteria.Done(entityMap, dbAdapter);
            var commandSpec = entityMap.GetListCommand(criteria.Parameters, criteria.Sql);
            return List<T>(commandSpec);
        }

        public IList<T> List<T>() where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            var commandSpec = entityMap.GetListAllCommand();
            return List<T>(commandSpec);
        }

        public T Get<T>(object id) where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            return List(entityMap.GetGetCommand(id)).Select(x => entityMap.BuildFrom(x, this)).FirstOrDefault();
        }

        public void SaveOrUpdate<T>(T entity) where T : class, new()
        {
            SaveOrUpdate(entity, null, null);
        }

        public void SaveOrUpdate<T>(T entity, string parentIdColumnName, object parentId) where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            var idMap = entityMap.PropertyMaps.Where(x => x is IIdPropertyMap<T>).Cast<IIdPropertyMap<T>>().Single();
            var commandSpec = entityMap.GetSaveCommand(entity, parentIdColumnName, parentId);
            ExecuteNonQuery(commandSpec);
            if (entityMap.IsTransient(entity) && idMap.Insert == false)
            {
                var getIdCommandSpec = dbAdapter.CreateLastInsertIdCommand();
                var getIdCommand = getIdCommandSpec.CreateCommand(dbAdapter, connection);
                var result = getIdCommand.ExecuteScalar();
                entityMap.SetId(entity, result, this);
            }
            Cascade(entityMap, entity);
        }

        private void Cascade<T>(IEntityMap<T> entityMap, T entity) where T : class, new()
        {
            var listMaps = entityMap.PropertyMaps.Where(x => x is IListPropertyMap<T>).Cast<IListPropertyMap<T>>();
            foreach (var map in listMaps)
            {
                map.Cascade(this, entity);
            }
        }

        public void Delete<T>(object id) where T : class, new()
        {
            var map = domainMap.GetMapFor<T>();
            var deleteCommand = map.GetDeleteCommand(id);
            ExecuteNonQuery(deleteCommand);
        }

        public void ExecuteNonQuery(DbCommandSpec commandSpec)
        {
            var command = commandSpec.CreateCommand(dbAdapter, connection);
            Try(command.ExecuteNonQuery);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteQuery(IDbCommand command)
        {
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader[i]);
                }
                yield return row;
            }
        }

        public object ExecuteScalar(DbCommandSpec commandSpec)
        {
            var command = commandSpec.CreateCommand(dbAdapter, connection);
            return Try(command.ExecuteScalar);
        }

        public T ExecuteScalar<T>(DbCommandSpec commandSpec)
        {
            var command = commandSpec.CreateCommand(dbAdapter, connection);
            var result = Try(command.ExecuteScalar);
            return (T)result;
        }

        public void RollbackTransaction()
        {
            transaction.Rollback();
        }

        public object ConvertFromDbType(object value, Type type)
        {
            return dbAdapter.ConvertFromDbType(value, type);
        }

        public string ToSql<T>(ICriteria<T> criteria) where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            criteria.Done(entityMap, dbAdapter);
            return criteria.Sql;
        }

        public IList<IDictionary<string, object>> GetTableMetaData(string tableName)
        {
            return List(dbAdapter.CreateGetTableMetadataCommand(tableName));
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Commit();
            }
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        private T Try<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                Log.Error(ex);
                throw;
            }
        }
    }
}