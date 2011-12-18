using System;
using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public static class Domain
    {
        public static DomainMap Map { get; private set; }

        public static void Configure(Action<IDomainMappable> config)
        {
            Map = new DomainMap();
            config(Map);
            Map.Done();
        }
    }
}