using System;
using System.Collections.Generic;
using System.Threading;

namespace Catnap
{
    public class ThreadSafeCache<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(); 
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public bool Contains(TKey key)
        {
            return InReadLock(() => dictionary.ContainsKey(key));
        }

        public TValue Get(TKey key)
        {
            return InReadLock(() => dictionary[key]);
        }

        public void Add(TKey key, TValue value)
        {
            InWriteLock(() => dictionary.Add(key, value));
        }
        
        public void Remove(TKey key)
        {
            InWriteLock(() => dictionary.Remove(key));
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