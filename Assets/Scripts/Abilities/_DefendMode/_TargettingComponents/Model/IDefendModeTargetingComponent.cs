using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingComponent
    {
        IReadOnlyList<IDefendModeTargetable> FindTargets(
            IDefendingEntity attachedToDefendingEntity        
        );
    }
}