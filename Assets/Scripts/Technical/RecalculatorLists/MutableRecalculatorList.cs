namespace System.Collections.Generic
{
    public class MutableRecalculatorList<U, V, EA> : RecalculatorList<U, V> 
        where EA: EventArgs 
        where U : INotifyOnChange<EA>
    {
        public MutableRecalculatorList(
            List<U> list, 
            Func<List<U>, V> valueFunction
        ) : base(list, valueFunction)
        {
            foreach (U item in list)
            {
                item.OnChange += OnItemChange;
            }
        }

        public MutableRecalculatorList(Func<List<U>, V> valueFunction) : base(valueFunction)
        {
        }

        public override V Add(U item)
        {
            item.OnChange += OnItemChange;

            return base.Add(item);
        }

        public override V AddRange(IEnumerable<U> items)
        {
            foreach (U item in items)
            {
                item.OnChange += OnItemChange;
            }

            return base.AddRange(items);
        }

        public override V Remove(U item)
        {
            item.OnChange -= OnItemChange;

            return base.Remove(item);
        }

        private void OnItemChange(object sender, EA args)
        {
            CalculateValue();

            OnChangeVirtual(new EARecalculatorListChange<V>(Value));
        }
    }
}