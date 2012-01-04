using System;
using Catnap.Mapping.Impl;

namespace Catnap.Mapping.Conventions.Impl
{
    public class IdMappingConvention : IIdMappingConventionMappable
    {
        private readonly Func<IEntityMapDescriptor, string> propertyNameSpec;
        private Func<IEntityMapDescriptor, string> columnNameSpec;
        private IAccessStrategyFactory access;
        private IIdValueGenerator generator;

        public IdMappingConvention() : this(x => "Id") { }

        public IdMappingConvention(Func<IEntityMapDescriptor, string> propertyNameSpec)
        {
            this.propertyNameSpec = propertyNameSpec;
        }

        public IIdMappingConventionMappable Column(Func<IEntityMapDescriptor, string> columnNameSpec)
        {
            this.columnNameSpec = columnNameSpec;
            return this;
        }

        public IIdMappingConventionMappable Access(IAccessStrategyFactory access)
        {
            this.access = access;
            return this;
        }

        public IIdMappingConventionMappable Generator(IIdValueGenerator generator)
        {
            this.generator = generator;
            return this;
        }

        public IdPropertyMap<T, object> GetMap<T>(IEntityMapDescriptor entityMapDescriptor) where T : class, new()
        {
            var propertyName = propertyNameSpec(entityMapDescriptor);
            var columnName = columnNameSpec == null
                ? propertyName
                : columnNameSpec(entityMapDescriptor);
            return new IdPropertyMap<T, object>(propertyName).ColumnName(columnName).Access(access).Generator(generator);
        }
    }
}