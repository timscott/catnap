namespace Catnap.Find
{
    public interface ICondition
    {
        object Left { get; }
        object Right { get; }
        string Operator { get; }
    }
}