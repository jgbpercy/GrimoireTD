namespace System.Collections.Generic
{
    public class MutableRecalculatorList<TItem, TValue, TEventArgs> : RecalculatorList<TItem, TValue> 
        where TEventArgs : EventArgs 
        where TItem : INotifyOnChange<TEventArgs>
    {
        public MutableRecalculatorList(
            List<TItem> list, 
            Func<List<TItem>, TValue> valueFunction
        ) : base(list, valueFunction)
        {
            foreach (var item in list)
            {
                item.OnChange += OnItemChange;
            }
        }

        public MutableRecalculatorList(Func<List<TItem>, TValue> valueFunction) : base(valueFunction)
        {
        }

        public override TValue Add(TItem item)
        {
            item.OnChange += OnItemChange;

            return base.Add(item);
        }

        public override TValue AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                item.OnChange += OnItemChange;
            }

            return base.AddRange(items);
        }

        public override TValue Remove(TItem item)
        {
            item.OnChange -= OnItemChange;

            return base.Remove(item);
        }

        private void OnItemChange(object sender, TEventArgs args)
        {
            CalculateValue();

            OnChangeVirtual(new EAOnRecalculatorListChange<TValue>(Value));
        }
    }
}