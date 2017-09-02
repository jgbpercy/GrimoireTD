using System;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Abilities
{
    public class EAOnDefendModeAbilityAdded : EventArgs
    {
        public readonly IDefendModeAbility DefendModeAbility;

        public EAOnDefendModeAbilityAdded(IDefendModeAbility defendModeAbility)
        {
            DefendModeAbility = defendModeAbility;
        }
    }
}