namespace Catnap.Mapping
{
    public interface IBelongsToPropertyMap : IBelongsToPropertyMapDescriptor
    {
        void Done(IEntityMap map);
    }
}