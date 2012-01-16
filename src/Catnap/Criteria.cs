using Catnap.Citeria.Conditions;

namespace Catnap
{
    public static class Criteria
    {
        public static ICriteria<T> For<T>() where T : class, new()
        {
            return new Criteria<T>();
        }
    }
}