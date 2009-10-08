using System;
using System.Collections.Generic;
using System.Linq;

namespace Catnap.Maps
{
    public static class DomainMap
    {
        private static IDictionary<Type, IEntityMap> entityMaps = new Dictionary<Type, IEntityMap>();

        public static void Configure(params IEntityMap[] maps)
        {
            foreach (var map in maps)
            {
                entityMaps.Add(map.EntityType, map);
            }
        }

        public static IEntityMap<T> GetMapFor<T>() where T : class, IEntity, new()
        {
            return (IEntityMap<T>)entityMaps.Where(x => x.Key == typeof(T)).First().Value;
        }
    }
}