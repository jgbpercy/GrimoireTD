using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public class FloatRangeArgs : DefendModeTargetingArgs
    {
        public readonly float Range;

        public FloatRangeArgs(
            IDefender attachedToDefender,
            float range
        ) : base(attachedToDefender)
        {
            Range = range;
        }
    }
}