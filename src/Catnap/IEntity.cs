using System;

namespace Catnap
{
    [Obsolete]
    public interface IEntity
    {
        int Id { get; }
        bool IsTransient { get; }
        void SetId(int id);
    }
}