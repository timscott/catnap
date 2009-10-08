using Catnap.Common;

namespace Catnap
{
    public interface IQuerySpec<T> where T : class, IEntity, new()
    {
        DbCommandSpec ToCommand();
    }
}