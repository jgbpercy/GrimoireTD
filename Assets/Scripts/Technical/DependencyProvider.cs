using System;
using GrimoireTD.Abilities;
using GrimoireTD.Abilities.DefendMode;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Dependencies
{
    public static class DependencyProvider
    {
        public static Func<
            IAbilities,
            IDefendingEntity,
            IDefendModeAbilityManager
        > 
            DefendModeAbilityManager = (
                abilities,
                defendingEntity
            ) =>
        {
            return new CDefendModeAbilityManager(
                abilities,
                defendingEntity
            );
        };
    }
}