using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.BuildMode
{
    public abstract class CBuildModeEffectComponent : IBuildModeEffectComponent
    {
        private IBuildModeEffectComponentTemplate buildModeEffectComponentTemplate;

        public CBuildModeEffectComponent(IBuildModeEffectComponentTemplate buildModeEffectComponentTemplate)
        {
            this.buildModeEffectComponentTemplate = buildModeEffectComponentTemplate;
        }

        public abstract void ExecuteEffect(IDefendingEntity executingEntity, IReadOnlyList<IBuildModeTargetable> targets);
    }
}