using System;
using System.Collections.Generic;

namespace GrimoireTD.Technical
{
    /* Possible optimisation: on list modification set a flag to recalc when accessed. 
     * However these will likely be things in views that will be accessed upon modification anyway
     * 
     * T Collection contents type
     * U Return value type
     */
    public class RecalculatorList<U, V>
    {
        private List<U> internalList;

        private Func<List<U>, V> valueFunction;

        private V value;

        public V Value
        {
            get
            {
                return value;
            }
        } 

        public RecalculatorList(List<U> list, Func<List<U>, V> valueFunction)
        {
            internalList = list;
            this.valueFunction = valueFunction;

            CalculateValue();
        }

        public RecalculatorList(Func<List<U>, V> valueFunction) : this(new List<U>(), valueFunction)
        {
        }

        public V Add(U item)
        {
            internalList.Add(item);

            CalculateValue();

            return value;
        }

        public V AddRange(IEnumerable<U> items)
        {
            internalList.AddRange(items);

            CalculateValue();

            return value;
        }

        public V Remove(U item)
        {
            internalList.Remove(item);

            CalculateValue();

            return value;
        }

        private void CalculateValue()
        {
            value = valueFunction(internalList);
        }
    }
}