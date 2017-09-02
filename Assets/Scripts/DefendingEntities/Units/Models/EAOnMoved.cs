using System;
using GrimoireTD.Map;

namespace GrimoireTD.DefendingEntities.Units
{
    public class EAOnMoved : EventArgs
    {
        public readonly Coord ToPosition;

        public EAOnMoved(Coord toPosition)
        {
            ToPosition = toPosition;
        }
    }
}