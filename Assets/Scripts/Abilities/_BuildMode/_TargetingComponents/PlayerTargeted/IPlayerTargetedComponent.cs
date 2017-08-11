using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetedComponent
    {
        bool IsValidTarget(DefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget);
    }
}
