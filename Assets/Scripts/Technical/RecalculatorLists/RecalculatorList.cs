namespace System.Collections.Generic
{
    public class RecalculatorList<TItem, TValue> : IReadOnlyRecalculatorList<TValue>, INotifyOnChange<EAOnRecalculatorListChange<TValue>>
    {
        private List<TItem> internalList;

        private Func<List<TItem>, TValue> valueFunction;

        public TValue Value { get; private set; }

        public event EventHandler<EAOnRecalculatorListChange<TValue>> OnChange;

        public RecalculatorList(List<TItem> list, Func<List<TItem>, TValue> valueFunction)
        {
            internalList = list;
            this.valueFunction = valueFunction;

            CalculateValue();
        }

        public RecalculatorList(Func<List<TItem>, TValue> valueFunction) : this(new List<TItem>(), valueFunction)
        {
        }

        public virtual TValue Add(TItem item)
        {
            internalList.Add(item);

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<TValue>(Value));

            return Value;
        }

        public virtual TValue AddRange(IEnumerable<TItem> items)
        {
            internalList.AddRange(items);

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<TValue>(Value));

            return Value;
        }

        public virtual TValue Remove(TItem item)
        {
            var found = internalList.Remove(item);

            if (!found) throw new KeyNotFoundException("Attempted to remove non-existent item from RecalculatorList");

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<TValue>(Value));

            return Value;
        }

        //TODO? RemoveRange

        public virtual TValue Replace(TItem outgoingItem, TItem incomingItem)
        {
            var found = internalList.Remove(outgoingItem);

            if (!found) throw new KeyNotFoundException("Attempted to replace a non-existent item in RecalculatorList");

            internalList.Add(incomingItem);

            CalculateValue();

            OnChange?.Invoke(this, new EAOnRecalculatorListChange<TValue>(Value));

            return Value;
        }

        //TODO? ReplaceRange

        protected void CalculateValue()
        {
            Value = valueFunction(internalList);
        }

        //see: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
        protected virtual void OnChangeVirtual(EAOnRecalculatorListChange<TValue> eventArgs)
        {
            EventHandler<EAOnRecalculatorListChange<TValue>> tempCopy = OnChange;

            tempCopy?.Invoke(this, eventArgs);
        }
    }
}