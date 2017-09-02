using System;

namespace GrimoireTD.Attributes
{
    public class EAOnAttributeChanged : EventArgs
    {
        public readonly float NewValue;

        public EAOnAttributeChanged(float newValue)
        {
            NewValue = newValue;
        }
    }
}