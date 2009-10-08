using System;

namespace Catnap
{
    public class UnitOfWork : IDisposable
    {
        private static UnitOfWork current;
        private bool isStarted;

        public ISession Session { get; private set; }

        public static UnitOfWork Start()
        {
            if (current != null && current.isStarted)
            {
                throw new InvalidOperationException("Cannot start current unit of work because it is already started.");
            }
            current = new UnitOfWork { Session = SessionFactory.New() };
            current.Session.Open();
            current.isStarted = true;
            return current;
        }

        public static UnitOfWork Current
        {
            get
            {
                if (current == null)
                {
                    throw new Exception("Unit of work not started.  You must start the unit of work before using it.");
                }
                return current;
            }
        }

        public void Dispose()
        {
            Session.Dispose();
            current = null;
            isStarted = false;
        }
    }
}