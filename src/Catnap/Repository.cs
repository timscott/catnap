using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Find.Conditions;

namespace Catnap
{
    public abstract class Repository<T> : IRepository<T> where T : class, new() 
    {
        protected readonly IRepository innerRepository;

        protected Repository() : this(new Repository()) { }

        protected Repository(IRepository innerRepository)
        {
            this.innerRepository = innerRepository;
        }

        public IList<T> List()
        {
            return innerRepository.List<T>();
        }

        public IList<T> List(ICriteria<T> criteria)
        {
            return innerRepository.List(criteria);
        }

        public virtual IList<T> List(Expression<Func<T, bool>> predicate)
        {
            return innerRepository.List(predicate);
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

        public IList<T> List<T>() where T : class, new()
        {
            return UnitOfWork.Current.Session.List<T>();
        }

        public IList<T> List<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var criteria = Criteria.For<T>().Where(predicate);
            return UnitOfWork.Current.Session.List(criteria);
        }

        public IList<T> List<T>(ICriteria<T> criteria) where T : class, new()
        {
            return UnitOfWork.Current.Session.List(criteria);
        }
    }
}