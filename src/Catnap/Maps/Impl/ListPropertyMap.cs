using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Catnap.Find;

namespace Catnap.Maps.Impl
{
    public class ListPropertyMap<TEntity, TListMember> : BasePropertyMap<TEntity, IEnumerable<TListMember>>, IListPropertyMap<TEntity>
        where TEntity : class, new()
        where TListMember : class, new()
    {
        private readonly Expression<Func<TListMember, bool>> filter;
        private IEntityMap listItemMap;
        private IEntityMap parentMap;

        public ListPropertyMap(Expression<Func<TEntity, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascadeDeletes) 
            : this(property, isLazy, cascadeSaves, cascadeDeletes, null) { }

        public ListPropertyMap(Expression<Func<TEntity, IEnumerable<TListMember>>> property, bool isLazy)
            : this(property, isLazy, true, true, null) { }

        public ListPropertyMap(Expression<Func<TEntity, IEnumerable<TListMember>>> property, bool isLazy, bool cascadeSaves, bool cascadeDeletes, Expression<Func<TListMember, bool>> filter) 
            : base(property, Access.Property)
        {
            this.filter = filter;
            IsLazy = isLazy;
            WillCascadeSaves = cascadeSaves;
            WillCascadeDeletes = cascadeDeletes;
        }

        public bool IsLazy { get; private set; }
        public bool WillCascadeSaves { get; private set; }
        public bool WillCascadeDeletes { get; private set; }

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

        public void SetMaps(IEntityMap parentMap, IEntityMap listItemMap)
        {
            this.listItemMap = listItemMap;
            this.parentMap = parentMap;
        }

        private void CascadeSaves(ISession session, TEntity parent, IEnumerable<TListMember> list)
        {
            if (!WillCascadeSaves)
            {
                return;
            }
            foreach (var item in list)
            {
                session.SaveOrUpdate(item, parentMap.GetId(parent));
            }
        }

        private void CascadeDeletes(ISession session, IEnumerable<TListMember> itemsToDelete)
        {
            if (!WillCascadeDeletes)
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
                .AddCondition(listItemMap.ParentColumnName, "=", parentMap.GetId(parent));
            if (filter != null)
            {
                builder.AddCondition(filter);
            }
            return session.List<TListMember>(builder.Build());
        }

        protected override void InnerSetValue(TEntity instance, object value, ISession session)
        {
            if (IsLazy)
            {
                var proxy = new LazyList<TListMember>(() => Load(session, instance));
                accessStrategy.Setter(instance, proxy);
            }
            else
            {
                base.InnerSetValue(instance, Load(session, instance), session);
            }
        }
    }
}