namespace Catnap.Maps
{
    public interface IIdMappingConventionMappable
    {
        IIdMappingConventionMappable Column(string name);
        IIdMappingConventionMappable Access(IAccessStrategyFactory access);
    }
}