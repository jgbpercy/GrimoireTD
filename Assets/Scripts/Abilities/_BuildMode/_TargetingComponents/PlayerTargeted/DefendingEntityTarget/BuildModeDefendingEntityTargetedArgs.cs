using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class BuildModeDefendingEntityTargetedArgs
    {
        public readonly IDefendingEntity SourceEntity;

        public readonly IDefendingEntity TargetEntity;

        public readonly IReadOnlyMapData MapData;

        public BuildModeDefendingEntityTargetedArgs(
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