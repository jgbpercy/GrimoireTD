using GrimoireTD.DefendingEntities;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetedComponent
    {
        bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget, IReadOnlyMapData mapData);
    }
}
