using System;

namespace GrimoireTD.Attributes
{
    public class EAOnAnyAttributeChanged<T> : EventArgs where T : struct, IConvertible
    {
        public readonly T AttributeName;

        public readonly float NewValue;

        public EAOnAnyAttributeChanged(T attributeName, float newValue)
        {
            AttributeName = attributeName;
            NewValue = newValue;
        }
    }
}