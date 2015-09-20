using System;
using System.Collections;
using System.Collections.Generic;

namespace DoorPrize.Generic
{
    public abstract class AbstractList<T> : IEnumerable<T>
    {
        protected readonly List<T> Items = new List<T>();

        public int Count
        {
            get { return Items.Count; }
        }

        public T this[int index]
        {
            get { return Items[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Items.ForEach(action);
        }
    }
}