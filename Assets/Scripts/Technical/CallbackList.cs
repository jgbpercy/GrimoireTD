using System.Linq;

namespace System.Collections.Generic
{
    public class CallbackList<T> : IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
    {
        private List<T> list;

        private Action<T> OnAdd;
        private Action<T> OnRemove;

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

            OnAdd?.Invoke(item);
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

            OnRemove?.Invoke(item);

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

            OnRemove?.Invoke(matchingItem);

            return true;
        }

        public void Clear()
        {
            while (list.Count > 0)
            {
                T item = list[0];

                list.RemoveAt(0);

                OnRemove(item);
            }
        }

        public void RegisterForAdd(Action<T> callback)
        {
            OnAdd += callback;
        }

        public void DeregisterForAdd(Action<T> callback)
        {
            OnAdd -= callback;
        }

        public void RegisterForRemove(Action<T> callback)
        {
            OnRemove += callback;
        }

        public void DeregisterForRemove(Action<T> callback)
        {
            OnRemove -= callback;
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