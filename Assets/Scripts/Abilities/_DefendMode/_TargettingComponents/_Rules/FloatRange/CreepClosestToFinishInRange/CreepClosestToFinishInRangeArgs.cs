using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CreepClosestToFinishInRangeArgs : FloatRangeArgs
    {
        public CreepClosestToFinishInRangeArgs(
            IDefender attachedToDefender,
            float baseRange
        ) : base(attachedToDefender, baseRange)
        {

        }
    }
}