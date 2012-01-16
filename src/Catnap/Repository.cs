using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Citeria.Conditions;

namespace Catnap
{
    [Obsolete]
    public abstract class Repository<T> : IRepository<T> where T : class, new() 
    {
        private ISession Session
        {
            get { return UnitOfWork.Current.Session; }
        }

        public IList<T> Find()
        {
            return Session.List<T>();
        }

        public IList<T> Find(ICriteria<T> criteria)
        {
            return Session.List(criteria);
        }

        public virtual IList<T> Find(Expression<Func<T, bool>> predicate)
        {
            return Session.List(predicate);
        }

        public virtual void Save(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public virtual T Get(object id)
        {
            return Session.Get<T>(id);
        }

        public virtual void Delete(object id)
        {
            Session.Delete<T>(id);
        }
    }
}