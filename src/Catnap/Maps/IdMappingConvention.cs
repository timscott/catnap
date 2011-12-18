using Catnap.Maps.Impl;

namespace Catnap.Maps
{
    public class IdMappingConvention
    {
        private IAccessStrategyFactory access;
        private string columnName;
        public readonly string propertyName;

        public IdMappingConvention() : this("Id") { }

        public IdMappingConvention(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public IdMappingConvention Column(string name)
        {
            columnName = name;
            return this;
        }

        public IdMappingConvention Access(IAccessStrategyFactory access)
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