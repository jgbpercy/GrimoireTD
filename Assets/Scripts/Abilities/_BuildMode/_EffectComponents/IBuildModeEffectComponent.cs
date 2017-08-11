using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public interface IBuildModeEffectComponent
    {
        void ExecuteEffect(DefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets);
    }
}
