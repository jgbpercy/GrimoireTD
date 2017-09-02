using System;

namespace GrimoireTD.Economy
{
    public class EAOnResourceChanged : EventArgs
    {
        public readonly int ByAmount;

        public readonly int ToAmount;

        public EAOnResourceChanged(int byAmount, int toAmount)
        {
            ByAmount = byAmount;
            ToAmount = toAmount;
        }
    }
}