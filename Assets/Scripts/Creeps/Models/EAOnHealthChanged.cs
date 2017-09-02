using System;

namespace GrimoireTD.Creeps
{
    public class EAOnHealthChanged : EventArgs
    {
        public readonly int NewValue;

        public EAOnHealthChanged(int newValue)
        {
            NewValue = newValue;
        }
    }
}