using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetedComponent : IBuildModeTargetingComponent
    {
        bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget);
    }
}
