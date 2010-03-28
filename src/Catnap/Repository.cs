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

        public virtual T Get(int id)
        {
            return innerRepository.Get<T>(id);
        }

        public virtual void Delete(int id)
        {
            innerRepository.Delete<T>(id);
        }

        protected void DeleteCollection<TList>(IEnumerable<TList> collection, IEntityMap map)
            where TList : class, IEntity, new()
        {
            foreach (var entity in collection)
            {
                innerRepository.Delete<T>(entity.Id);
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

    public class Repository : IRepository
    {
        public T Get<T>(int id) where T : class, IEntity, new()
        {
            return UnitOfWork.Current.Session.Get<T>(id);
        }

        public void Save<T>(T entity) where T : class, IEntity, new()
        {
            UnitOfWork.Current.Session.SaveOrUpdate(entity);
        }

        public void Delete<T>(int id) where T : class, IEntity, new()
        {
            UnitOfWork.Current.Session.Delete<T>(id);
        }

        public IEnumerable<T> Find<T>() where T : class, IEntity, new()
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().Build());
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class, IEntity, new()
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().AddCondition(predicate).Build());
        }

        public IEnumerable<T> Find<T>(ICriteria criteria) where T : class, IEntity, new()
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().AddCriteria(criteria).Build());
        }
    }
}