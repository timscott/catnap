using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Catnap.Mapping.Impl
{
    public class ListPropertyMap<TEntity, TListMember> : BasePropertyMap<TEntity, IEnumerable<TListMember>, ListPropertyMap<TEntity, TListMember>>, IListPropertyMap<TEntity>, IListPropertyMappable<TEntity, TListMember> 
        where TEntity : class, new()
        where TListMember : class, new()
    {
        private Expression<Func<TListMember, bool>> filter;
        private IEntityMap<TListMember> listItemMap;
        private IEntityMap parentMap;
        private string parentIdColumnName;
        private bool willCascadeSaves;
        private bool willCascadeDeletes;
        private bool isLazy;

        public ListPropertyMap(Expression<Func<TEntity, IEnumerable<TListMember>>> property)
            : base(property)
        {
            Lazy(true);
            CascadeSaves(true);
            CascadeDeletes(true);
        }

        public IListPropertyMappable<TEntity, TListMember> Lazy(bool value)
        {
            isLazy = value;
            return this;
        }

        public IListPropertyMappable<TEntity, TListMember> CascadeSaves(bool value)
        {
            willCascadeSaves = value;
            return this;
        }

        public IListPropertyMappable<TEntity, TListMember> CascadeDeletes(bool value)
        {
            willCascadeDeletes = value;
            return this;
        }

        public IListPropertyMappable<TEntity, TListMember> Filter(Expression<Func<TListMember, bool>> value)
        {
            filter = value;
            return this;
        }

        public IListPropertyMappable<TEntity, TListMember> ParentIdColumn(string columnName)
        {
            parentIdColumnName = columnName;
            return this;
        }

        public void Cascade(ISession session, TEntity parent)
        {
            var existingList = Load(session, parent);
            var list = accessStrategy.Getter(parent).ToList();
            var itemsToDelete = existingList.Except(list, new EntityEqualityComaparer<TListMember>(listItemMap));
            CascadeDeletes(session, itemsToDelete);
            CascadeSaves(session, parent, list);
        }

        public Type ItemType
        {
            get { return typeof(TListMember); }
        }

        public Type ParentType
        {
            get { return typeof(TEntity); }
        }

        public void Done(IDomainMap domainMap, IEntityMap parentMap, IEntityMap listItemMap)
        {
            //TODO: This casting smells a little funny.
            this.listItemMap = (IEntityMap<TListMember>)listItemMap;
            this.parentMap = parentMap;
            parentIdColumnName = parentIdColumnName ?? GetDeafultParentIdColumnColumnName(domainMap);
        }

        private void CascadeSaves(ISession session, TEntity parent, IEnumerable<TListMember> list)
        {
            if (!willCascadeSaves)
            {
                return;
            }
            foreach (var item in list)
            {
                session.SaveOrUpdate(item, parentIdColumnName, parentMap.GetId(parent));
            }
        }

        private void CascadeDeletes(ISession session, IEnumerable<TListMember> itemsToDelete)
        {
            if (!willCascadeDeletes)
            {
                return;
            }
            foreach (var item in itemsToDelete)
            {
                session.Delete<TListMember>(parentMap.GetId(item));
            }
        }

        public IList<TListMember> Load(ISession session, TEntity parent)
        {
            var criteria = Criteria.For<TListMember>().Equal(parentIdColumnName, parentMap.GetId(parent));
            if (filter != null)
            {
                criteria = criteria.Where(filter);
            }
            return session.List(criteria);
        }

        protected override void InnerSetValue(TEntity instance, object value, ISession session)
        {
            if (isLazy)
            {
                var proxy = new LazyList<TListMember>(() => Load(session, instance));
                accessStrategy.Setter(instance, proxy);
            }
            else
            {
                var list = Load(session, instance);
                base.InnerSetValue(instance, list, session);
            }
        }

        protected string GetDeafultParentIdColumnColumnName(IDomainMap domainMap)
        {
            return domainMap.ListParentIdColumnNameMappingConvention == null
                ? parentMap.EntityType.Name + "Id"
                : domainMap.ListParentIdColumnNameMappingConvention.GetColumnName(this);
        }
    }
}