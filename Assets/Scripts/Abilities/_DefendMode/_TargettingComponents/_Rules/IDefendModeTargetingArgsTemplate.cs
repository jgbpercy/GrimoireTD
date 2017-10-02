using System.Collections.Generic;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetingArgsTemplate
    {
        DefendModeTargetingArgs GenerateArgs(
            IDefendingEntity attachedToDefendingEntity,
            IReadOnlyList<ICreep> creepManager
        );
    }
}