using System;

namespace GrimoireTD.Abilities.DefendMode
{
    public class EAOnAbilityOffCooldown : EventArgs
    {
        public readonly IDefendModeAbility DefendModeAbility;

        public EAOnAbilityOffCooldown(IDefendModeAbility defendModeAbility)
        {
            DefendModeAbility = defendModeAbility;
        }
    }
}