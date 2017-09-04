using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public class BuildModeHexTargetedArgs
    {
        public readonly IDefendingEntity SourceEntity;

        public readonly Coord TargetCoord;

        public readonly IReadOnlyMapData MapData;

        public BuildModeHexTargetedArgs(
            IDefendingEntity sourceEntity, 
            Coord targetCoord, 
            IReadOnlyMapData mapData
        )
        {
            SourceEntity = sourceEntity;
            TargetCoord = targetCoord;
            MapData = mapData;
        }
    }
}