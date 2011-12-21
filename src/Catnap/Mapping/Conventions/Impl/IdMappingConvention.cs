using Catnap.Mapping.Impl;

namespace Catnap.Mapping.Conventions.Impl
{
    public class IdMappingConvention : IIdMappingConventionMappable
    {
        private IAccessStrategyFactory access;
        private string columnName;
        public readonly string propertyName;

        public IdMappingConvention() : this("Id") { }

        public IdMappingConvention(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public IIdMappingConventionMappable Column(string name)
        {
            columnName = name;
            return this;
        }

        public IIdMappingConventionMappable Access(IAccessStrategyFactory access)
        {
            this.access = access;
            return this;
        }

        public IdPropertyMap<T, object> GetMap<T>() where T : class, new()
        {
            return new IdPropertyMap<T, object>(propertyName).ColumnName(columnName ?? propertyName).Access(access);
        }
    }
}