using System;
using GrimoireTD.Map;

namespace GrimoireTD.Defenders.Units
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