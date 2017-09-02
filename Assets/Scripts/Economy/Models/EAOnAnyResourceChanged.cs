using System;

namespace GrimoireTD.Economy
{
    public class EAOnAnyResourceChanged : EventArgs
    {
        public readonly IReadOnlyResource Resource;

        public readonly int ByAmount;

        public readonly int ToAmount;

        public EAOnAnyResourceChanged(IReadOnlyResource resouce, int byAmount, int toAmount)
        {
            Resource = resouce;
            ByAmount = byAmount;
            ToAmount = toAmount;
        }
    }
}