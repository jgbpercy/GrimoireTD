using System;

namespace GrimoireTD.Creeps
{
    public class EAOnBlockChanged : EventArgs
    {
        public readonly int NewValue;

        public EAOnBlockChanged(int newValue)
        {
            NewValue = newValue;
        }
    }
}