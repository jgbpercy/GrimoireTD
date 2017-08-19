using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetedComponent
    {
        bool IsValidTarget(IDefendingEntity sourceDefendingEntity, IBuildModeTargetable potentialTarget);
    }
}
