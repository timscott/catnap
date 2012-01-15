using System;

namespace Catnap
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }
    }
}