using System;
using GrimoireTD.Map;

namespace GrimoireTD.Defenders.Units
{
    public class EAOnMoved : EventArgs
    {
        public readonly IUnit Unit;

        public readonly Coord OldPosition;

        public readonly Coord ToPosition;

        public EAOnMoved(IUnit unit, Coord oldPosition, Coord toPosition)
        {
            Unit = unit;
            ToPosition = toPosition;
            OldPosition = oldPosition;
        }
    }
}