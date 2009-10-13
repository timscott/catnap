namespace Catnap.Maps
{
    public static class Domain
    {
        public static DomainMap Map { get; private set; }

        public static void Configure(params IEntityMap[] entityMaps)
        {
            Map = new DomainMap(entityMaps);
        }
    }
}