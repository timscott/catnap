using System;
using System.Threading;
using Catnap.Logging;

namespace Catnap
{
    public class UnitOfWork : IUnitOfWork
    {
        private static ISessionFactory sessionFactory;
        [ThreadStatic]
        private static UnitOfWork current;

        public static void Initialize(ISessionFactory sessionFactory)
        {
            UnitOfWork.sessionFactory = sessionFactory;
        }

        public static UnitOfWork Current
        {
            get
            {
                if (current == null)
                {
                    throw new InvalidOperationException("Unit of work not started.  You must start the unit of work before using it.");
                }
                return current;
            }
        }

        private Guid id;

        public ISession Session { get; private set; }

        public static UnitOfWork Start()
        {
            if (current != null)
            {
                throw new InvalidOperationException(string.Format("Cannot start current unit of work {0} because it is already started. Thread: {1}", current.id, Thread.CurrentThread.ManagedThreadId));
            }
            current = new UnitOfWork
            {
                Session = sessionFactory.New(), 
                id = Guid.NewGuid()
            };
            Log.Debug("Starting unit of work {0}. Thread: {1}", current.id, Thread.CurrentThread.ManagedThreadId);
            current.Session.Open();
            return current;
        }

        public void Dispose()
        {
            Log.Debug("Disposing unit of work {0}. Thread: {1}", current.id, Thread.CurrentThread.ManagedThreadId);
            Session.Dispose();
            current = null;
        }
    }
}