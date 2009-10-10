using Catnap.Common;

namespace Catnap.Find
{
    public interface IFindSpec<T> where T : class, IEntity, new()
    {
        DbCommandSpec ToCommand();
    }
}