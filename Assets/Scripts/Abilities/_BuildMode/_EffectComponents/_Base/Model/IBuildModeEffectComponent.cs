using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeEffectComponent
    {
        void ExecuteEffect(IDefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets);
    }
}