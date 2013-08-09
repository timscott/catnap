using System;
using System.Collections.Generic;
using System.Linq;
using Catnap.Database;
using Catnap.Extensions;
using Catnap.Mapping.Conventions;
using Catnap.Mapping.Conventions.Impl;

namespace Catnap.Mapping.Impl
{
    public class DomainMap : IDomainMap, IDomainMappable
    {
        private IDbAdapter dbAdapter;
        // -- Changed by RD to overcome Monotouch limitations when Value types are used as Dictionary Keys
        // -- see http://docs.xamarin.com/ios/guides/advanced_topics/limitations
        private Dictionary<string, IEntityMap> entityMaps = new Dictionary<string, IEntityMap>();	// - RD

        public DomainMap(IDbAdapter dbAdapter)
        {
            dbAdapter.GuardArgumentNull("dbAdapter");
            this.dbAdapter = dbAdapter;
        }

        public IdMappingConvention IdMappingConvention { get; private set; }
        public BelongsToColumnNameConvention BelongsToColumnNameMappingConvention { get; private set; }
        public ListParentIdColumnNameConvention ListParentIdColumnNameMappingConvention { get; private set; }

        public IEntityMappable<T> Entity<T>(Action<IEntityMappable<T>> propertyMappings) where T : class, new()
        {
            if (entityMaps.ContainsKey(typeof(T).Name)) // RD - change
            {
                throw new ApplicationException(string.Format("Cannot map type '{0}' because it is already mapped.", typeof(T)));
            }
            var map = new EntityMap<T>(propertyMappings);
            entityMaps.Add(typeof(T).Name, map);
            return map;
        }

        public IIdMappingConventionMappable IdConvention()
        {
            IdMappingConvention = new IdMappingConvention();
            return IdMappingConvention;
        }

        public IIdMappingConventionMappable IdConvention(Func<IEntityMapDescriptor, string> propertyNameSpec)
        {
            IdMappingConvention = new IdMappingConvention(propertyNameSpec);
            return IdMappingConvention;
        }

        public void BelongsToColumnNameConvention(Func<IBelongsToPropertyMapDescriptor, string> convention)
        {
            BelongsToColumnNameMappingConvention = new BelongsToColumnNameConvention(convention);
        }

        public void ListParentIdColumnNameConvention(Func<IListPropertyMapDescriptor, string> convention)
        {
            ListParentIdColumnNameMappingConvention = new ListParentIdColumnNameConvention(convention);
        }

        public IEntityMap<T> GetMapFor<T>() where T : class, new()
        {
            return (IEntityMap<T>)entityMaps.First(x => x.Key == typeof(T).Name).Value; // RD - change
        }

        public IEntityMap GetMapFor(Type type)
        {
            return entityMaps.First(x => x.Key == type.Name).Value;  // RD - change
        }

        // -- RD wrapped done calls with exception handling - probably not needed.
        public void Done ()
        {
            if (IdMappingConvention == null)
            {
	            IdMappingConvention = new IdMappingConvention ();
            }
            foreach (var map in entityMaps.Values)
            {
	            try
	            {
		            map.Done(this, dbAdapter);
	            }
	            catch (Exception e)
	            {
		            Console.WriteLine("FAILED Done for " + map.TableName + " " + e);
	            }

            }
        }
    }
}