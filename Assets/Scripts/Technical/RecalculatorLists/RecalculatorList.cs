namespace System.Collections.Generic
{
    public class RecalculatorList<U, V> : IReadOnlyRecalculatorList<V>, INotifyOnChange<EARecalculatorListChange<V>>
    {
        private List<U> internalList;

        private Func<List<U>, V> valueFunction;

        public V Value { get; private set; }

        public event EventHandler<EARecalculatorListChange<V>> OnChange;

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

            OnChange?.Invoke(this, new EARecalculatorListChange<V>(Value));

            return Value;
        }

        public virtual V AddRange(IEnumerable<U> items)
        {
            internalList.AddRange(items);

            CalculateValue();

            OnChange?.Invoke(this, new EARecalculatorListChange<V>(Value));

            return Value;
        }

        public virtual V Remove(U item)
        {
            internalList.Remove(item);

            CalculateValue();

            OnChange?.Invoke(this, new EARecalculatorListChange<V>(Value));

            return Value;
        }

        protected void CalculateValue()
        {
            Value = valueFunction(internalList);
        }

        //see: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
        protected virtual void OnChangeVirtual(EARecalculatorListChange<V> eventArgs)
        {
            EventHandler<EARecalculatorListChange<V>> tempCopy = OnChange;

            tempCopy?.Invoke(this, eventArgs);
        }
    }
}