using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Catnap.Maps;

namespace Catnap
{
    public abstract class Repository<T> : IRepository<T> where T : class, IEntity, new() 
    {
        private IEntityMap<T> entityMap;

        protected Repository()
        {
            entityMap = DomainMap.GetMapFor<T>();
        }

        public IEnumerable<T> Find()
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().Build());
        }

        public virtual IEnumerable<T> Find(IFindSpec<T> findSpec)
        {
            return UnitOfWork.Current.Session.List<T>(findSpec.ToCommand());
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return UnitOfWork.Current.Session.List<T>(new FindCommandBuilder<T>().AddCondition(predicate).Build());
        }

        public virtual void Save(T entity)
        {
            UnitOfWork.Current.Session.Save(entity);
        }

        public virtual T Get(int id)
        {
            return UnitOfWork.Current.Session.Get<T>(id);
        }

        public virtual void Delete(int id)
        {
            UnitOfWork.Current.Session.Delete<T>(id);
        }

        protected void DeleteCollection<T>(IEnumerable<T> collection, IEntityMap map)
            where T : class, IEntity, new()
        {
            foreach (var entity in collection)
            {
                UnitOfWork.Current.Session.Delete<T>(entity.Id);
            }
        }

        protected void SaveCollection<T>(int parentId, IEnumerable<T> collection, IEntityMap map)
            where T : class, IEntity, new()
        {
            foreach (var entity in collection)
            {
                UnitOfWork.Current.Session.Save(entity, parentId);
            }
        }
    }
}