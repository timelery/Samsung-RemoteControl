using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartTVRemoteControl
{
    public class ThreadSafeCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        //private ICollection<T> collection;
        private readonly ICollection<T> collection = new List<T>();
        private readonly object collectionLock = new Object();

        public int Count
        {
            get
            {
                int count;
                lock (this.collectionLock)
                {
                    if (collection == null || collection.Count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        count = collection.Count;
                    }

                }
                return count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                bool isReadOnly;
                lock (this.collectionLock)
                {
                    isReadOnly = this.collection.IsReadOnly;
                }
                return isReadOnly;
            }
        }

        public ThreadSafeCollection()
        {
        }

        public void Add(T item)
        {
            lock (this.collectionLock)
            {
                this.collection.Add(item);
            }
        }

        public void Clear()
        {
            lock (this.collectionLock)
            {
                this.collection.Clear();
            }
        }

        public bool Contains(T item)
        {
            bool flag;
            lock (this.collectionLock)
            {
                flag = this.collection.Contains(item);
            }
            return flag;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.collectionLock)
            {
                this.collection.CopyTo(array, arrayIndex);
            }
        }

        public T Find(Func<T, bool> predicate)
        {
            T t;
            lock (this.collectionLock)
            {
                t = this.collection.Where<T>(predicate).FirstOrDefault<T>();
            }
            return t;
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerator<T> enumerator;
            lock (this.collectionLock)
            {
                enumerator = this.collection.GetEnumerator();
            }
            return enumerator;
        }

        public bool Remove(T item)
        {
            bool flag;
            lock (this.collectionLock)
            {
                flag = this.collection.Remove(item);
            }
            return flag;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            IEnumerator enumerator;
            lock (this.collectionLock)
            {
                enumerator = this.collection.GetEnumerator();
            }
            return enumerator;
        }
    }
}