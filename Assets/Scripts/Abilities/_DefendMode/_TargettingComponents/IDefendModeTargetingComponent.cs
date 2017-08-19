using System.Collections.Generic;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingComponent
    {
        IReadOnlyList<IDefendModeTargetable> FindTargets(IDefendingEntity attachedToDefendingEntity);
    }
}