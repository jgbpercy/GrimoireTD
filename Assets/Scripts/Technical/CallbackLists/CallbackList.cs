using System.Linq;

namespace System.Collections.Generic
{
    public class CallbackList<T> : IReadOnlyCallbackList<T>
    {
        private List<T> list;

        public event EventHandler<EAOnCallbackListAdd<T>> OnAdd;
        public event EventHandler<EAOnCallbackListRemove<T>> OnRemove;

        public IEnumerable<T> AsIEnumarable
        {
            get
            {
                return list;
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public CallbackList()
        {
            list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);

            OnAdd?.Invoke(this, new EAOnCallbackListAdd<T>(item));
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public bool TryRemove(T item)
        {
            if (!list.Contains(item))
            {
                return false;
            }          

            list.Remove(item);

            OnRemove?.Invoke(this, new EAOnCallbackListRemove<T>(item));

            return true;
        }

        public bool TryRemove(T item, IEqualityComparer<T> comparer)
        {
            var matchingItem = list.FirstOrDefault(x => comparer.Equals(x, item));

            if (matchingItem == null)
            {
                return false;
            }

            list.Remove(matchingItem);

            OnRemove?.Invoke(this, new EAOnCallbackListRemove<T>(matchingItem));

            return true;
        }

        public void Clear()
        {
            while (list.Count > 0)
            {
                var item = list[0];

                list.RemoveAt(0);

                OnRemove?.Invoke(this, new EAOnCallbackListRemove<T>(item));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}