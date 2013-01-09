using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Citeria.Conditions;
using Catnap.Database;
using Catnap.Exceptions;
using Catnap.Extensions;
using Catnap.Logging;
using Catnap.Mapping;

namespace Catnap
{
    public class Session : ISession
    {
        private readonly IDbConnection connection;
        private readonly IDomainMap domainMap;
        private readonly IDbCommandFactory commandFactory;
        private readonly ISessionCache sessionCache;
        private IDbTransaction transaction;
        private bool wasDisposed;

        public Session(IDomainMap domainMap, IDbConnection connection, IDbCommandFactory commandFactory, IDbAdapter dbAdapter, ISessionCache sessionCache)
        {
            this.domainMap = domainMap;
            this.commandFactory = commandFactory;
            DbAdapter = dbAdapter;
            this.sessionCache = sessionCache;
            this.connection = connection;
        }

        public IDbAdapter DbAdapter { get; private set; }

        public void Open()
        {
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public IList<IDictionary<string, object>> List(IDbCommandSpec commandSpec)
        {
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec.Parameters, commandSpec.CommandText);
            var result = Try(() => ExecuteQuery(command));
            command.Dispose (); //RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return result;
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
            var results = ExecuteQuery(command).Select(x => entityMap.BuildFrom(x, this)).ToList();
            StoreAll(entityMap, results);
            command.Dispose ();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return results;
        }

        public IList<T> List<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var criteria = Criteria.For<T>().Where(predicate);
            return List(criteria);
        }

        public IList<T> List<T>() where T : class, new()
        {
            var entityMap = domainMap.GetMapFor<T>();
            var command = entityMap.GetListAllCommand(commandFactory);
            var results = ExecuteQuery(command).Select(x => entityMap.BuildFrom(x, this)).ToList();
            StoreAll(entityMap, results);
            command.Dispose();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return results;
        }

        public T Get<T>(object id) where T : class, new()
        {
            id.GuardArgumentNull("id");
            var cached = sessionCache.Retrieve<T>(id);
            if (cached != null)
            {
                return cached;
            }
            var entityMap = domainMap.GetMapFor<T>();
            var command = entityMap.GetGetCommand(id, commandFactory);
            var result = ExecuteQuery(command).Select(x => entityMap.BuildFrom(x, this)).FirstOrDefault();
            sessionCache.Store(id, result);
            command.Dispose ();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return result;
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
                var getIdCommand = DbAdapter.CreateLastInsertIdCommand(entityMap.TableName, commandFactory);
                var result = getIdCommand.ExecuteScalar();
                entityMap.SetId(entity, result, this);
            }
            sessionCache.Store(entityMap.GetId(entity), entity);
            Cascade(entityMap, entity);
            command.Dispose();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
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
            sessionCache.Store<T>(id, null);
            deleteCommand.Dispose ();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
        }

        public void ExecuteNonQuery(IDbCommandSpec commandSpec)
        {
            GuardNotDisposed();
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec);
            Try(command.ExecuteNonQuery);
            command.Dispose ();	//RD
        }

        public IList<IDictionary<string, object>> ExecuteQuery(IDbCommand command)
        {
            GuardNotDisposed();
            command.GuardArgumentNull("command");
            var result = new List<IDictionary<string, object>>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader[i]);
                    }
                    result.Add(row);
                }
                reader.Close();
                command.Dispose ();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
                return result;
            }
        }

        public object ExecuteScalar(IDbCommandSpec commandSpec)
        {
            GuardNotDisposed();
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec);
            object result = Try(command.ExecuteScalar);
            command.Dispose();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return result;
        }

        public T ExecuteScalar<T>(IDbCommandSpec commandSpec)
        {
            GuardNotDisposed();
            commandSpec.GuardArgumentNull("commandSpec");
            var command = commandFactory.Create(commandSpec);
            var result = Try(command.ExecuteScalar);
            command.Dispose ();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return (T)result;
        }

        public bool TableExists(string tableName)
        {
            var getTableMetadataCommand = DbAdapter.CreateGetTableMetadataCommand(tableName, commandFactory);
            var result = ExecuteQuery(getTableMetadataCommand);
            getTableMetadataCommand.Dispose ();	//RD see http://www.aaronheise.com/2012/12/monotouch-sqlite-sigsegv/
            return result.Count > 0;
        }

        public object ConvertFromDbType(object value, Type type)
        {
            return DbAdapter.ConvertFromDb(value, type);
        }

        public IEntityMap<T> GetEntityMapFor<T>() where T : class, new()
        {
            return domainMap.GetMapFor<T>();
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
            wasDisposed = true;
        }

        private T Try<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                transaction = null;
                Log.Error(ex);
                throw;
            }
        }

        private void StoreAll<T>(IEntityMap entityMap, List<T> results) where T : class, new()
        {
            results.ForEach(x => sessionCache.Store(entityMap.GetId(x), x));
        }

        private void GuardNotDisposed()
        {
            if (wasDisposed)
            {
                throw new SessionDisposedException();
            }
        }
    }
}