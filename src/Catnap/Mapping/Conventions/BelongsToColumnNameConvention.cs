using System;
using Catnap.Mapping.Impl;

namespace Catnap.Mapping.Conventions
{
    public class BelongsToColumnNameConvention
    {
        private readonly Func<IBelongsToPropertyMap, string> convention;

        public BelongsToColumnNameConvention(Func<IBelongsToPropertyMap, string> convention)
        {
            this.convention = convention;
        }

        public string GetColumnName<TEntity, TProperty>(BelongsToPropertyMap<TEntity, TProperty> map) 
            where TEntity : class, new()
            where TProperty : class, new()
        {
            return convention(map);
        }
    }
}