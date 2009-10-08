using Catnap.Common;

namespace Catnap
{
    public interface IFindSpec<T> where T : class, IEntity, new()
    {
        DbCommandSpec ToCommand();
    }
}