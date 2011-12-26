namespace Catnap.Mapping
{
    //TODO: Get rid of this as a separate ssingleton.  Move into session factory.
    internal static class Domain
    {
        internal static IDomainMap Map { get; private set; }

        internal static void Initialize(IDomainMap map)
        {
            Map = map;
        }
    }
}