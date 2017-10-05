using System;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Abilities
{
    public class EAOnAbilityExecuted : EventArgs
    {
        public readonly IDefendModeAbility DefendModeAbility;

        public EAOnAbilityExecuted(IDefendModeAbility defendModeAbility)
        {
            DefendModeAbility = defendModeAbility;
        }
    }
}