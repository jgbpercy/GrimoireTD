using System;

namespace GrimoireTD.Creeps
{
    public class EAOnResistanceChanged : EventArgs
    {
        public readonly float NewValue;

        public EAOnResistanceChanged(float newValue)
        {
            NewValue = newValue;
        }
    }
}