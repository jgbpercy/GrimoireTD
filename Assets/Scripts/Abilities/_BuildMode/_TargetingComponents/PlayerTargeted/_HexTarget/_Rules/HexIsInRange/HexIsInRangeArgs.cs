using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class HexIsInRangeArgs : PlayerTargetsHexArgs
    {
        public readonly int Range;

        public HexIsInRangeArgs(
            IDefendingEntity sourceEntity, 
            Coord targetCoord, 
            int range
        ) : base(sourceEntity, targetCoord)
        {
            Range = range;
        }
    }
}