using System;
using System.Linq.Expressions;

namespace Catnap.Mapping.Impl
{
    public class IdPropertyMap<TEntity, TProperty> : PropertyWithColumnMap<TEntity, TProperty, IdPropertyMap<TEntity, TProperty>>, IIdPropertyMap<TEntity>, IIdPropertyMappable<TEntity, TProperty, IdPropertyMap<TEntity, TProperty>> where TEntity : class, new()
    {
        private IIdValueGenerator generator;

        public IdPropertyMap(string propertyName) : base(propertyName) { }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property) : base(property) { }

        public IdPropertyMap<TEntity, TProperty> Generator(IIdValueGenerator value)
        {
            generator = value;
            return this;
        }

        public override bool Insert
        {
            get { return generator != null; }
        }

        public object Generate(TEntity entity)
        {
            var id = generator.Generate();
            accessStrategy.Setter(entity, (TProperty)id);
            return id;
        }
    }
}