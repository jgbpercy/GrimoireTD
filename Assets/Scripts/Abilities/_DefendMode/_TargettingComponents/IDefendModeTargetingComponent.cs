using System.Collections.Generic;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingComponent
    {
        IDefendModeTargetingArgsTemplate TargetingRule { get; }

        IReadOnlyList<IDefendModeTargetable> FindTargets(IDefendingEntity attachedToDefendingEntity, IReadOnlyCreepManager creepManager);
    }
}