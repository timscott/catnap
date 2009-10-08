namespace Catnap
{
    public interface IEntity
    {
        int Id { get; }
        bool IsTransient { get; }
        void SetId(int id);
    }
}