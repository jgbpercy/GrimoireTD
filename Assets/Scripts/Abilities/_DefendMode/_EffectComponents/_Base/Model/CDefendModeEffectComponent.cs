using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

//TODO: generic class combining build and defend mode effect components? Worth?
namespace GrimoireTD.Abilities.DefendMode
{
    public abstract class CDefendModeEffectComponent : IDefendModeEffectComponent
    {
        private IDefendModeEffectComponentTemplate defendModeEffectComponentTemplate;

        public CDefendModeEffectComponent(IDefendModeEffectComponentTemplate defendModeEffectComponentTemplate)
        {
            this.defendModeEffectComponentTemplate = defendModeEffectComponentTemplate;
        }

        public abstract void ExecuteEffect(IDefendingEntity attachedToDefendingEntity, IReadOnlyList<IDefendModeTargetable> targets);
    }
}