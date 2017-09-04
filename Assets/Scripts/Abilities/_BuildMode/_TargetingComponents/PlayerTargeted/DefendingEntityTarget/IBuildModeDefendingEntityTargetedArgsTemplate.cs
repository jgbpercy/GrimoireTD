using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeDefendingEntityTargetedArgsTemplate
    {
        BuildModeDefendingEntityTargetedArgs GenerateArgs(
            IDefendingEntity sourceEntity, 
            IDefendingEntity targetEntity, 
            IReadOnlyMapData mapData
        );
    }
}