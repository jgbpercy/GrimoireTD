using System.Collections.Generic;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeEffectComponent
    {
        void ExecuteEffect(IDefender executingDefender, IReadOnlyList<IBuildModeTargetable> targets);
    }
}