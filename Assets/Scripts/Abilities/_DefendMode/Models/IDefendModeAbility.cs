using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using System;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeAbility : IAbility
    {
        float ActualCooldown { get; }

        IDefendModeAbilityTemplate DefendModeAbilityTemplate { get; }

        string Id { get; }

        float PercentOfCooldownPassed { get; }

        float TimeSinceExecuted { get; }
        float TimeSinceExecutedClamped { get; }

        bool IsOffCooldown { get; }

        event EventHandler<EAOnAbilityOffCooldown> OnAbilityOffCooldown;

        //TODO: make readonly defendingentity?
        bool ExecuteAbility(IDefendingEntity attachedToDefendingEntity);
    }
}