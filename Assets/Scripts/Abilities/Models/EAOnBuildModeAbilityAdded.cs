using System;

namespace GrimoireTD.Abilities.BuildMode
{
    public class EAOnBuildModeAbilityAdded : EventArgs
    {
        public readonly IBuildModeAbility BuildModeAbility;

        public EAOnBuildModeAbilityAdded(IBuildModeAbility buildModeAbility)
        {
            BuildModeAbility = buildModeAbility;
        }
    }
}