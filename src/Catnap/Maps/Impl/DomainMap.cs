using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public class DomainMap : IDomainMap
    {
        private IDictionary<Type, IEntityMap> entityMaps = new Dictionary<Type, IEntityMap>();

        public DomainMap(params IEntityMap[] entityMaps)
        {
            Log.Debug("Mapping domain.");
            foreach (var map in entityMaps)
            {
                this.entityMaps.Add(map.EntityType, map);
            }
            foreach (var map in entityMaps)
            {
                map.Done(this);
            }
        }

        public IEntityMap<T> GetMapFor<T>() where T : class, IEntity, new()
        {
            return (IEntityMap<T>)entityMaps.Where(x => x.Key == typeof(T)).First().Value;
        }

        public IEntityMap GetMapFor(Type type)
        {
            return entityMaps.Where(x => x.Key == type).First().Value; 
        }
    }
}