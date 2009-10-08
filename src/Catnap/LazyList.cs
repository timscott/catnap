using System;
using System.Collections;
using System.Collections.Generic;

namespace Catnap
{
    public class LazyList<T> : IEnumerable<T>
    {
        private readonly Func<IList<T>> loader;
        private IList<T> innerList;
        private readonly object locker = new object();

        public LazyList(Func<IList<T>> loader)
        {
            this.loader = loader;
        }

        private IList<T> InnerList
        {
            get
            {
                if (innerList == null)
                {
                    lock (locker)
                    {
                        if (innerList == null)
                        {
                            innerList = loader();
                        }
                    }
                }
                return innerList;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        public void Add(T item)
        {
            InnerList.Add(item);
        }
    }
}