namespace Catnap.Mapping.Conventions
{
    public interface IIdMappingConventionMappable
    {
        IIdMappingConventionMappable Column(string name);
        IIdMappingConventionMappable Access(IAccessStrategyFactory access);
    }
}