using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CreepClosestToFinishInRangeArgs : FloatRangeArgs
    {
        public readonly IReadOnlyCreepManager CreepManager;

        public CreepClosestToFinishInRangeArgs(
            IDefendingEntity attachedToDefendingEntity,
            float baseRange,
            IReadOnlyCreepManager creepManager
        ) : base(
            attachedToDefendingEntity,
            baseRange
        )
        {
            CreepManager = creepManager;
        }
    }
}