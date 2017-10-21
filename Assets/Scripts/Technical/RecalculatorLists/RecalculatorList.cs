namespace System.Collections.Generic
{
    public class RecalculatorList<U, V> : IReadOnlyRecalculatorList<V>, INotifyOnChange<EAOnRecalculatorListChange<V>>
    {
        private List<U> internalList;

        private Func<List<U>, V> valueFunction;

        public V Value { get; private set; }

        public event EventHandler<EAOnRecalculatorListChange<V>> OnChange;

        public RecalculatorList(List<U> list, Func<List<U>, V> valueFunction)
        {
            internalList = list;
            this.valueFunction = valueFunction;

            CalculateValue();
        }

        public RecalculatorList(Func<List<U>, V> valueFunction) : this(new List<U>(), valueFunction)
        {
        }

        public virtual V Add(U item)
        {
            internalList.Add(item);

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<V>(Value));

            return Value;
        }

        public virtual V AddRange(IEnumerable<U> items)
        {
            internalList.AddRange(items);

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<V>(Value));

            return Value;
        }

        public virtual V Remove(U item)
        {
            var found = internalList.Remove(item);

            if (!found) throw new KeyNotFoundException("Attempted to remove non-existent item from RecalculatorList");

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<V>(Value));

            return Value;
        }

        //TODO? RemoveRange

        public virtual V Replace(U outgoingItem, U incomingItem)
        {
            var found = internalList.Remove(outgoingItem);

            if (!found) throw new KeyNotFoundException("Attempted to replace a non-existent item in RecalculatorList");

            internalList.Add(incomingItem);

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<V>(Value));

            return Value;
        }

        //TODO? ReplaceRange

        protected void CalculateValue()
        {
            Value = valueFunction(internalList);
        }

        //see: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
        protected virtual void OnChangeVirtual(EAOnRecalculatorListChange<V> eventArgs)
        {
            EventHandler<EAOnRecalculatorListChange<V>> tempCopy = OnChange;

            tempCopy?.Invoke(this, eventArgs);
        }
    }
}