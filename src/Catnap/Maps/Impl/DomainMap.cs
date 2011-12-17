using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Common.Logging;

namespace Catnap.Maps.Impl
{
    public class DomainMap : IDomainMap, IDomainMappable
    {
        private readonly IDictionary<Type, IEntityMap> entityMaps = new Dictionary<Type, IEntityMap>();

        public DomainMap(params Func<IDomainMappable, IEntityMap>[] entityMaps)
        {
            Log.Debug("Mapping domain.");
            var maps = entityMaps.ToArray().Select(func => func(this)).ToArray();
            foreach (var map in maps)
            {
                this.entityMaps.Add(map.EntityType, map);
            }
            foreach (var map in maps)
            {
                map.Done(this);
            }
        }

        public IEntityMap<T> GetMapFor<T>() where T : class, new()
        {
            return (IEntityMap<T>)entityMaps.Where(x => x.Key == typeof(T)).First().Value;
        }

        public IEntityMap GetMapFor(Type type)
        {
            return entityMaps.Where(x => x.Key == type).First().Value; 
        }
        
        public IEntityMappable<T> Entity<T>(params Func<IEntityMappable<T>, IPropertyMap<T>>[] propertyMaps) where T : class, new()
        {
            return new EntityMap<T>(propertyMaps);
        }
    }

    public interface IDomainMappable
    {
        IEntityMappable<T> Entity<T>(params Func<IEntityMappable<T>, IPropertyMap<T>>[] propertyMaps) where T : class, new();
    }
}