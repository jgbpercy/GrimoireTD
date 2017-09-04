using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public abstract class CBuildModeEffectComponent : IBuildModeEffectComponent
    {
        public CBuildModeEffectComponent() { }

        public abstract void ExecuteEffect(IDefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets);
    }
}