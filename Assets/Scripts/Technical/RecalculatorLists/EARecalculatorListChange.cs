namespace System.Collections.Generic
{
    public class EAOnRecalculatorListChange<V> : EventArgs
    {
        public readonly V NewValue;

        public EAOnRecalculatorListChange(V newValue)
        {
            NewValue = newValue;
        }
    }
}