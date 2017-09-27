using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class PlayerTargetsDefendingEntityArgs
    {
        public readonly IDefendingEntity SourceEntity;

        public readonly IDefendingEntity TargetEntity;

        public readonly IReadOnlyMapData MapData;

        public PlayerTargetsDefendingEntityArgs(
            IDefendingEntity sourceEntity,
            IDefendingEntity targetEntity,
            IReadOnlyMapData mapData
        )
        {
            SourceEntity = sourceEntity;
            TargetEntity = targetEntity;
            MapData = mapData;
        }
    }
}