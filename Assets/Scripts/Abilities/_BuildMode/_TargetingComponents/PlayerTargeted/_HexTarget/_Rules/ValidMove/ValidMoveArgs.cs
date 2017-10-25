using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class ValidMoveArgs : PlayerTargetsHexArgs
    {
        public readonly int Range;

        public ValidMoveArgs(
            IDefendingEntity sourceEntity,
            Coord targetCoord,
            int range
        ) : base(sourceEntity, targetCoord)
        {
            Range = range;
        }
    }
}