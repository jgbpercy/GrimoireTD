using System;

namespace GrimoireTD.Attributes
{
    public interface IReadOnlyAttributes<T> where T : struct, IConvertible
    {
        event EventHandler<EAOnAnyAttributeChanged<T>> OnAnyAttributeChanged;

        IReadOnlyAttribute GetAttribute(T attributeName);
    }
}