using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public class FloatRangeArgs : DefendModeTargetingArgs
    {
        public readonly float Range;

        public FloatRangeArgs(
            IDefendingEntity attachedToDefendingEntity,
            float range
        ) : base(attachedToDefendingEntity)
        {
            Range = range;
        }
    }
}