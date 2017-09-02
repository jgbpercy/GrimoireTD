namespace System.Collections.Generic
{
    public class EARecalculatorListChange<V> : EventArgs
    {
        public readonly V NewValue;

        public EARecalculatorListChange(V newValue)
        {
            NewValue = newValue;
        }
    }
}