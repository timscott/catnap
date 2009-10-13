using System;
using System.Collections.Generic;
using System.Linq;

namespace Catnap.Maps
{
    public class DomainMap : IDomainMap
    {
        private IDictionary<Type, IEntityMap> entityMaps = new Dictionary<Type, IEntityMap>();

        public DomainMap(params IEntityMap[] entityMaps)
        {
            foreach (var map in entityMaps)
            {
                this.entityMaps.Add(map.EntityType, map);
            }
        }

        public IEntityMap<T> GetMapFor<T>() where T : class, IEntity, new()
        {
            return (IEntityMap<T>)entityMaps.Where(x => x.Key == typeof(T)).First().Value;
        }  
    }
}