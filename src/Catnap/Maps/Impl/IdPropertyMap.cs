using System;
using System.Linq.Expressions;
using Catnap.Common.Logging;
using Catnap.Extensions;

namespace Catnap.Maps.Impl
{
    public abstract class Generator
    {
        public static Generator GuidNew = new GuidGenerator();
        public static Generator GuidComb = new GuidCombGenerator();

        public abstract object Generate();

        public class GuidCombGenerator : Generator
        {
            public override object Generate()
            {
                return Guid.NewGuid();  //TODO: Implement COMB
            }
        }

        public class GuidGenerator : Generator
        {
            public override object Generate()
            {
                return Guid.NewGuid();
            }
        }
    }

    public class IdPropertyMap<TEntity, TProperty> : BasePropertyMap<TEntity, TProperty>, IIdPropertyMap<TEntity>
        where TEntity : class, new()
    {
        private readonly Generator generator;

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property) : this(property, Access.CamelCaseField) { }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, Access access) : this(property, null, access) { }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, Access access, Generator generator) : this(property, null, access, generator) { }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName, Access access) : this(property, columnName, access, null) { }

        public IdPropertyMap(string propertyName, string columnName, Access access) : base(propertyName, access)
        {
            Log.Debug("Setting column name for Value property '{0}'.", propertyName);
            ColumnName = columnName ?? accessStrategy.PropertyInfo.Name;
        }

        public IdPropertyMap(Expression<Func<TEntity, TProperty>> property, string columnName, Access access, Generator generator) : base(property, access)
        {
            Log.Debug("Setting column name for Value property '{0}'.", property.GetMemberExpression().Member.Name);
            ColumnName = columnName ?? accessStrategy.PropertyInfo.Name;
            this.generator = generator;
        }

        public string ColumnName { get; private set; }

        public bool Insert
        {
            get { return generator != null; }
        }

        public Type PropertyType
        {
            get { return accessStrategy.PropertyInfo.PropertyType; }
        }

        public object GetValue(TEntity instance)
        {
            return accessStrategy.Getter(instance);
        }

        public object Generate(TEntity entity)
        {
            var id = generator.Generate();
            accessStrategy.Setter(entity, (TProperty)id);
            return id;
        }
    }
}