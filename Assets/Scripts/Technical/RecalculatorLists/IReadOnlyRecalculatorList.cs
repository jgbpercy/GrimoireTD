namespace System.Collections.Generic
{
    public interface IReadOnlyRecalculatorList<V>
    {
        V Value { get; }

        event EventHandler<EARecalculatorListChange<V>> OnChange;
    }
}