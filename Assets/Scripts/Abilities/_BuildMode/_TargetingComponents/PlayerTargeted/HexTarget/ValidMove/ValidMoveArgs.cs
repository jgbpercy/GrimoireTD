using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class ValidMoveArgs : BuildModeHexTargetedArgs
    {
        public readonly int Range;

        public ValidMoveArgs(
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