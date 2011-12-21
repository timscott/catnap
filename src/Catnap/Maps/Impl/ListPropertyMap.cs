using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Find;

namespace Catnap.Maps.Impl
{
    public class ListPropertyMap<TEntity, TListMember> : BasePropertyMap<TEntity, IEnumerable<TListMember>, ListPropertyMap<TEntity, TListMember>>, IListPropertyMap<TEntity>
        where TEntity : class, new()
        where TListMember : class, new()
    {
        private Expression<Func<TListMember, bool>> filter;
        private IEntityMap listItemMap;
        private IEntityMap parentMap;
        private bool isLazy;
        private bool willCascadeSaves;
        private bool willCascadeDeletes;
        private string parentIdColumnName;

        public ListPropertyMap(Expression<Func<TEntity, IEnumerable<TListMember>>> property) : base(property)
        {
            Lazy(true);
            CascadeSaves(true);
            CascadeDeletes(true);
        }

        public ListPropertyMap<TEntity, TListMember> Lazy(bool value)
        {
            isLazy = value;
            return this;
        }

        public ListPropertyMap<TEntity, TListMember> CascadeSaves(bool value)
        {
            willCascadeSaves = value;
            return this;
        }

        public ListPropertyMap<TEntity, TListMember> CascadeDeletes(bool value)
        {
            willCascadeDeletes = value;
            return this;
        }

        public ListPropertyMap<TEntity, TListMember> Filter(Expression<Func<TListMember, bool>> value)
        {
            filter = value;
            return this;
        }

        public bool GetIsLazy()
        {
            return isLazy;
        }

        public bool GetWillCascadeSaves()
        {
            return willCascadeSaves;
        }

        public bool GetWillCascadeDeletes()
        {
            return willCascadeDeletes;
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
            this.listItemMap = listItemMap;
            this.parentMap = parentMap;
            parentIdColumnName = parentIdColumnName ?? GetDeafultParentIdColumnColumnName(domainMap);
        }

        private void CascadeSaves(ISession session, TEntity parent, IEnumerable<TListMember> list)
        {
            if (!GetWillCascadeSaves())
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
            if (!GetWillCascadeDeletes())
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
            var builder = new FindCommandBuilder<TListMember>()
                .AddCondition(parentIdColumnName, "=", parentMap.GetId(parent));
            if (filter != null)
            {
                builder.AddCondition(filter);
            }
            return session.List<TListMember>(builder.Build());
        }

        protected override void InnerSetValue(TEntity instance, object value, ISession session)
        {
            if (GetIsLazy())
            {
                var proxy = new LazyList<TListMember>(() => Load(session, instance));
                accessStrategy.Setter(instance, proxy);
            }
            else
            {
                base.InnerSetValue(instance, Load(session, instance), session);
            }
        }

        public void ParentIdColumn(string columnName)
        {
            parentIdColumnName = columnName;
        }

        protected string GetDeafultParentIdColumnColumnName(IDomainMap domainMap)
        {
            return domainMap.ListParentIdColumnNameMappingConvention  == null
                ? parentMap.EntityType.Name + "Id"
                : domainMap.ListParentIdColumnNameMappingConvention.GetColumnName(this);
        }
    }
}