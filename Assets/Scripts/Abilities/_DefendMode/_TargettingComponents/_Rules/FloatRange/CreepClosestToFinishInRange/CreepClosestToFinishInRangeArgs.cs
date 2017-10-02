using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CreepClosestToFinishInRangeArgs : FloatRangeArgs
    {
        public readonly IReadOnlyList<ICreep> CreepList;

        public CreepClosestToFinishInRangeArgs(
            IDefendingEntity attachedToDefendingEntity,
            float baseRange,
            IReadOnlyList<ICreep> creepList
        ) : base(
            attachedToDefendingEntity,
            baseRange
        )
        {
            CreepList = creepList;
        }
    }
}