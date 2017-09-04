using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeHexTargetedArgsTemplate
    {
        BuildModeHexTargetedArgs GenerateArgs(
            IDefendingEntity sourceEntity,
            Coord targetCoord,
            IReadOnlyMapData mapData
        );
    }
}