using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public static class Domain
    {
        internal static DomainMap Map { get; private set; }

        public static void Configure(params IEntityMap[] entityMaps)
        {
            Map = new DomainMap(entityMaps);
        }
    }
}