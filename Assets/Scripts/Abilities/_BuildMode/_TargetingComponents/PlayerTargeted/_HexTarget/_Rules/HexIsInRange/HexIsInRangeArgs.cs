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
            IReadOnlyMapData mapData, 
            int range
        ) : base(sourceEntity, targetCoord, mapData)
        {
            Range = range;
        }
    }
}