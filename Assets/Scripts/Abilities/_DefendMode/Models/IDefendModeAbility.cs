using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeAbility : IAbility, IFrameUpdatee
    {
        float ActualCooldown { get; }

        IDefendModeAbilityTemplate DefendModeAbilityTemplate { get; }

        string Id { get; }

        float PercentOfCooldownPassed { get; }

        float TimeSinceExecuted { get; }
        float TimeSinceExecutedClamped { get; }

        //TODO: make readonly defendingentity?
        bool ExecuteAbility(IDefendingEntity attachedToDefendingEntity);

        void WasExecuted();

        bool OffCooldown();
    }
}