using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Find;
using Catnap.Find.Conditions;
using Catnap.Maps;

namespace Catnap
{
    public abstract class Repository<T> : IRepository<T> where T : class, IEntity, new() 
    {
        private IEntityMap<T> entityMap;

        protected Repository() : this(Domain.Map.GetMapFor<T>()) { }

        protected Repository(IEntityMap<T> entityMap)
        {
            this.entityMap = entityMap;
        }

        public IEnumerable<T> Find()
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().Build());
        }

        public IEnumerable<T> Find(ICriteria criteria)
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().AddCriteria(criteria).Build());
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().AddCondition(predicate).Build());
        }

        public virtual void Save(T entity)
        {
            UnitOfWork.Current.Session.SaveOrUpdate(entity);
        }

        public virtual T Get(int id)
        {
            return UnitOfWork.Current.Session.Get<T>(id);
        }

        public virtual void Delete(int id)
        {
            UnitOfWork.Current.Session.Delete<T>(id);
        }

        protected void DeleteCollection<TList>(IEnumerable<TList> collection, IEntityMap map)
            where TList : class, IEntity, new()
        {
            foreach (var entity in collection)
            {
                UnitOfWork.Current.Session.Delete<TList>(entity.Id);
            }
        }

        protected void SaveCollection<TList>(int parentId, IEnumerable<TList> collection, IEntityMap map)
            where TList : class, IEntity, new()
        {
            foreach (var entity in collection)
            {
                UnitOfWork.Current.Session.SaveOrUpdate(entity, parentId);
            }
        }
    }
}