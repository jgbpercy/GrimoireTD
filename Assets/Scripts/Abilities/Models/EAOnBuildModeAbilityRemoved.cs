using System;
using GrimoireTD.Abilities.BuildMode;

namespace GrimoireTD.Abilities
{
    public class EAOnBuildModeAbilityRemoved : EventArgs
    {
        public readonly IBuildModeAbility BuildModeAbility;

        public EAOnBuildModeAbilityRemoved(IBuildModeAbility buildModeAbility)
        {
            BuildModeAbility = buildModeAbility;
        }
    }
}