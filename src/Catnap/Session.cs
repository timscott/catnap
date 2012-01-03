using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Catnap.Citeria.Conditions;
using Catnap.Database;
using Catnap.Extensions;
using Catnap.Logging;
using Catnap.Mapping;

namespace Catnap
{
    public class Session : ISession
    {
        private readonly IDbConnection connection;
        private readonly IDomainMap domainMap;
        private readonly IDbAdapter dbAdapter;
        private readonly IDbCommandFactory commandFactory;
        private IDbTransaction transaction;

        public Session(IDomainMap domainMap, string connectionString, IDbAdapter dbAdapter)
        {
            this.domainMap = domainMap;
            this.dbAdapter = dbAdapter;
            connection = dbAdapter.CreateConnection(connectionString);
            commandFactory = new DbCommandFactory(dbAdapter, connection);
        }

        public void Open()
        {
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public IList<IDictionary<string, object>> List(IDbCommandSpec commandSpec)
        {
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec.Parameters, commandSpec.CommandText);
            return Try(() => ExecuteQuery(command)).ToList();
        }

        public IList<T> List<T>(IDbCommandSpec commandSpec) where T : class, new()
        {
            commandSpec.GuardArgumentNull("commandSpec");
            var entityMap = domainMap.GetMapFor<T>();
            return List(commandSpec).Select(x => entityMap.BuildFrom(x, this)).ToList();
        }

        public IList<T> List<T>(ICriteria<T> criteria) where T : class, new()
        {
            criteria.GuardArgumentNull("criteria");
            var entityMap = domainMap.GetMapFor<T>();
            var predicateSpec = criteria.Build(this);
            var command = entityMap.GetListCommand(predicateSpec.Parameters, predicateSpec.CommandText, commandFactory);
            return ExecuteQuery(command).Select(x => entityMap.BuildFrom(x, this)).ToList();
        }

        public IList<T> List<T>() where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            var command = entityMap.GetListAllCommand(commandFactory);
            return ExecuteQuery(command).Select(x => entityMap.BuildFrom(x, this)).ToList();
        }

        public T Get<T>(object id) where T : class, new()
        {
            id.GuardArgumentNull("id");
            var entityMap = domainMap.GetMapFor<T>();
            var command = entityMap.GetGetCommand(id, commandFactory);
            return ExecuteQuery(command).Select(x => entityMap.BuildFrom(x, this)).FirstOrDefault();
        }

        public void SaveOrUpdate<T>(T entity) where T : class, new()
        {
            SaveOrUpdate(entity, null, null);
        }

        public void SaveOrUpdate<T>(T entity, string parentIdColumnName, object parentId) where T : class, new()
        {
            entity.GuardArgumentNull("entity");
            var entityMap = domainMap.GetMapFor<T>();
            var idMap = entityMap.PropertyMaps.Where(x => x is IIdPropertyMap<T>).Cast<IIdPropertyMap<T>>().Single();
            var command = entityMap.GetSaveCommand(entity, parentIdColumnName, parentId, commandFactory);
            Try(command.ExecuteNonQuery);
            if (entityMap.IsTransient(entity) && idMap.Insert == false)
            {
                var getIdCommand = dbAdapter.CreateLastInsertIdCommand(entityMap.TableName, commandFactory);
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
            id.GuardArgumentNull("id");
            var map = domainMap.GetMapFor<T>();
            var deleteCommand = map.GetDeleteCommand(id, commandFactory);
            Try(deleteCommand.ExecuteNonQuery);
        }

        public void ExecuteNonQuery(IDbCommandSpec commandSpec)
        {
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec);
            Try(command.ExecuteNonQuery);
        }

        public IEnumerable<IDictionary<string, object>> ExecuteQuery(IDbCommand command)
        {
            command.GuardArgumentNull("command");
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

        public object ExecuteScalar(IDbCommandSpec commandSpec)
        {
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec);
            return Try(command.ExecuteScalar);
        }

        public T ExecuteScalar<T>(IDbCommandSpec commandSpec)
        {
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec);
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

        public IEntityMap<T> GetEntityMapFor<T>() where T : class, new()
        {
            return domainMap.GetMapFor<T>();
        }

        public IList<IDictionary<string, object>> GetTableMetaData(string tableName)
        {
            tableName.GuardArgumentNull("tableName");
            var getTableMetadataCommand = dbAdapter.CreateGetTableMetadataCommand(tableName, commandFactory);
            return Try(() => ExecuteQuery(getTableMetadataCommand)).ToList();
        }

        public string FormatParameterName(string name)
        {
            return dbAdapter.FormatParameterName(name);
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