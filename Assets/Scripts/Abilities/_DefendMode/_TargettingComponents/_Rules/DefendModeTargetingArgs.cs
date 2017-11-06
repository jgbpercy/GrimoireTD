using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public class DefendModeTargetingArgs
    {
        public readonly IDefender AttachedToDefender;

        public DefendModeTargetingArgs(IDefender attachedToDefender)
        {
            AttachedToDefender = attachedToDefender;
        }
    }
}