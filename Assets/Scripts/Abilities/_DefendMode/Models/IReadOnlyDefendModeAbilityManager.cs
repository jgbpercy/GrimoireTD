using System;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IReadOnlyDefendModeAbilityManager
    {
        event EventHandler<EAOnAllDefendModeAbilitiesOffCooldown> OnAllDefendModeAbilitiesOffCooldown;
    }
}