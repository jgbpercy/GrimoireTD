using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CreepClosestToFinishInRangeArgs : FloatRangeArgs
    {
        public CreepClosestToFinishInRangeArgs(
            IDefendingEntity attachedToDefendingEntity,
            float baseRange
        ) : base(attachedToDefendingEntity, baseRange)
        {

        }
    }
}