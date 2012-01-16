using System;

namespace Catnap.Migration
{
    public interface IDatabaseMigration
    {
        string Name { get; }
        Action<ISession> Action { get; }
    }
}