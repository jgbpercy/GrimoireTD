using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IPlayerTargetedComponent : IBuildModeTargetingComponent
    {
        bool IsValidTarget(IDefender sourceDefender, IBuildModeTargetable potentialTarget);
    }
}