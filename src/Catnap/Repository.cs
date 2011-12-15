using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Find;
using Catnap.Find.Conditions;
using Catnap.Maps;

namespace Catnap
{
    public abstract class Repository<T> : IRepository<T> where T : class, new() 
    {
        protected readonly IRepository innerRepository;
        protected readonly IEntityMap<T> entityMap;

        protected Repository() : this(new Repository(), Domain.Map.GetMapFor<T>()) { }

        protected Repository(IRepository innerRepository, IEntityMap<T> entityMap)
        {
            this.innerRepository = innerRepository;
            this.entityMap = entityMap;
        }

        public IEnumerable<T> Find()
        {
            return innerRepository.Find<T>();
        }

        public IEnumerable<T> Find(ICriteria criteria)
        {
            return innerRepository.Find<T>(criteria);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return innerRepository.Find(predicate);
        }

        public virtual void Save(T entity)
        {
            innerRepository.Save(entity);
        }

        public virtual T Get(object id)
        {
            return innerRepository.Get<T>(id);
        }

        public virtual void Delete(object id)
        {
            innerRepository.Delete<T>(id);
        }

        protected void DeleteCollection<TList>(IEnumerable<TList> collection, IEntityMap map)
            where TList : class, new()
        {
            foreach (var entity in collection)
            {
                innerRepository.Delete<T>(map.GetId(entity));
            }
        }

        protected void SaveCollection<TList>(object parentId, IEnumerable<TList> collection, IEntityMap map)
            where TList : class, new()
        {
            foreach (var entity in collection)
            {
                UnitOfWork.Current.Session.SaveOrUpdate(entity, parentId);
            }
        }
    }

    public class Repository : IRepository
    {
        public T Get<T>(object id) where T : class, new()
        {
            return UnitOfWork.Current.Session.Get<T>(id);
        }

        public void Save<T>(T entity) where T : class, new()
        {
            UnitOfWork.Current.Session.SaveOrUpdate(entity);
        }

        public void Delete<T>(object id) where T : class, new()
        {
            UnitOfWork.Current.Session.Delete<T>(id);
        }

        public IEnumerable<T> Find<T>() where T : class, new()
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().Build());
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var commandSpec = new FindCommandBuilder<T>().AddCondition(predicate).Build();
            return UnitOfWork.Current.Session.List<T>(commandSpec);
        }

        public IEnumerable<T> Find<T>(ICriteria criteria) where T : class, new()
        {
            var commandSpec = new FindCommandBuilder<T>().AddCriteria(criteria).Build();
            return UnitOfWork.Current.Session.List<T>(commandSpec);
        }
    }
}