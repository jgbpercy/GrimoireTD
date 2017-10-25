using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class PlayerTargetsHexArgs
    {
        public readonly IDefendingEntity SourceEntity;

        public readonly Coord TargetCoord;

        public PlayerTargetsHexArgs(
            IDefendingEntity sourceEntity, 
            Coord targetCoord
        )
        {
            SourceEntity = sourceEntity;
            TargetCoord = targetCoord;
        }
    }
}