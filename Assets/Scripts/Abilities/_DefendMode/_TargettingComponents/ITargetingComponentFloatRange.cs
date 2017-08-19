using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface ITargetingComponentFloatRange
    {
        float BaseRange { get; }

        float GetActualRange(IDefendingEntity attachedToDefendingEntity);
    }
}