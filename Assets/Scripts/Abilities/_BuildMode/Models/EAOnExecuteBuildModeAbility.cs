using System;

namespace GrimoireTD.Abilities.BuildMode
{
    public class EAOnExecutedBuildModeAbility : EventArgs
    {
        public readonly IBuildModeAbility ExecutedAbility;

        public EAOnExecutedBuildModeAbility(IBuildModeAbility executedAbility)
        {
            ExecutedAbility = executedAbility;
        }
    }
}