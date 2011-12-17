using System;
using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public static class Domain
    {
        public static IDomainMap Map { get; private set; }

        public static void Configure(IdMappingConvention idMappingConvention, params Func<IDomainMappable, IEntityMap>[] entityMaps)
        {
            Map = new DomainMap(idMappingConvention, entityMaps);
        }

        public static void Configure(params Func<IDomainMappable, IEntityMap>[] entityMaps)
        {
            Map = new DomainMap(null, entityMaps);
        }
    }
}