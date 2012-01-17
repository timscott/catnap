using System;
using System.Collections.Generic;
using System.Threading;

namespace Catnap
{
    public class SessionCache : ISessionCache
    {
        private readonly IDictionary<EntitySessionKey, object> dictionary = new Dictionary<EntitySessionKey, object>();
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public void Store<T>(object id, T entity) where T : class, new()
        {
            var key = new EntitySessionKey(typeof(T), id);
            if (InReadLock(() => dictionary.ContainsKey(key)))
            {
                InWriteLock(() => dictionary.Remove(key));
            }
            if (entity != null)
            {
                InWriteLock(() => dictionary.Add(key, entity));
            }
        }

        public T Retrieve<T>(object id) where T : class, new()
        {
            var key = new EntitySessionKey(typeof(T), id);
            return InReadLock(() => dictionary.ContainsKey(key))
                ? (T)InReadLock(() => dictionary[key])
                : default(T);
        }

        private T InReadLock<T>(Func<T> func)
        {
            cacheLock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        private void InWriteLock(Action action)
        {
            cacheLock.EnterReadLock();
            try
            {
                action();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }
    }
}