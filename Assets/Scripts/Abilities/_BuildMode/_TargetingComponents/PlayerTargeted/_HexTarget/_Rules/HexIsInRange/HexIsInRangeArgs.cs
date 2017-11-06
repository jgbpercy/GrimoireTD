using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class HexIsInRangeArgs : PlayerTargetsHexArgs
    {
        public readonly int Range;

        public HexIsInRangeArgs(
            IDefender sourceDefender, 
            Coord targetCoord, 
            int range
        ) : base(sourceDefender, targetCoord)
        {
            Range = range;
        }
    }
}