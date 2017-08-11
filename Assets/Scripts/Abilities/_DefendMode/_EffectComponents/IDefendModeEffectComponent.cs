using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeEffectComponent
    {
        void ExecuteEffect(DefendingEntity attachedToDefendingEntity, IReadOnlyList<IDefendModeTargetable> targets);
    }
}