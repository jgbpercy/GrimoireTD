using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public class DefendModeTargetingArgs
    {
        public readonly IDefendingEntity AttachedToDefendingEntity;

        public DefendModeTargetingArgs(IDefendingEntity attachedToDefendingEntity)
        {
            AttachedToDefendingEntity = attachedToDefendingEntity;
        }
    }
}