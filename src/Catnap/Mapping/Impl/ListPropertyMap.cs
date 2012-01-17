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
        private bool willCascadeSaves = true;
        private bool willCascadeDeletes = true;
        private bool isLazy = true;
        private IEqualityComparer<TListMember> listItemComparer;

        public ListPropertyMap(Expression<Func<TEntity, IEnumerable<TListMember>>> property) : base(property) { }

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
            var parentId = parentMap.GetId(parent);
            var persistedItems = Load(session, parentId);
            var currentItems = accessStrategy.Getter(parent).ToList();
            var toDeleteItems = persistedItems.Except(currentItems, listItemComparer);
            CascadeDeletes(session, toDeleteItems);
            CascadeSaves(session, parent, currentItems);
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
            listItemComparer = new EntityEqualityComaparer<TListMember>(listItemMap);
        }

        private void CascadeSaves(ISession session, TEntity parent, IEnumerable<TListMember> list)
        {
            if (!willCascadeSaves)
            {
                return;
            }
            foreach (var item in list)
            {
                var parentId = parentMap.GetId(parent);
                session.SaveOrUpdate(item, parentIdColumnName, parentId);
            }
        }

        private void CascadeDeletes(ISession session, IEnumerable<TListMember> itemsToDelete)
        {
            if (willCascadeDeletes)
            {
                return;
            }
            foreach (var item in itemsToDelete)
            {
                var itemId = listItemMap.GetId(item);
                session.Delete<TListMember>(itemId);
            }
        }

        protected override void InnerSetValue(TEntity instance, object parentId, ISession session)
        {
            IEnumerable<TListMember> list;
            if (isLazy)
            {
                list = new LazyList<TListMember>(() => Load(session, parentId));
            }
            else
            {
                list = Load(session, instance);
            }
            accessStrategy.Setter(instance, list);
        }

        private string GetDeafultParentIdColumnColumnName(IDomainMap domainMap)
        {
            return domainMap.ListParentIdColumnNameMappingConvention == null
                ? parentMap.EntityType.Name + "Id"
                : domainMap.ListParentIdColumnNameMappingConvention.GetColumnName(this);
        }

        private IList<TListMember> Load(ISession session, object parentId)
        {
            var criteria = Criteria.For<TListMember>().Equal(parentIdColumnName, parentId);
            if (filter != null)
            {
                criteria = criteria.Where(filter);
            }
            return session.List(criteria);
        }
    }
}