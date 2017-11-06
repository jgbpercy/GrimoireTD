using System;
using GrimoireTD.Defenders;
using GrimoireTD.Map;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeAbility : IAbility
    {
        IBuildModeAbilityTemplate BuildModeAbilityTemplate { get; }

        IBuildModeTargetingComponent TargetingComponent { get; }

        event EventHandler<EAOnExecutedBuildModeAbility> OnExecuted;

        void ExecuteAbility(IDefender executingDefender, Coord executionPosition);
    }
}