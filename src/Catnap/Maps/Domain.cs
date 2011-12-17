using System;
using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public static class Domain
    {
        public static IDomainMap Map { get; private set; }

        public static void Configure(params Func<IDomainMappable, IEntityMap>[] entityMaps)
        {
            Map = new DomainMap(entityMaps);
        }
    }
}