using System;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Abilities
{
    public class EAOnDefendModeAbilityRemoved : EventArgs
    {
        public readonly IDefendModeAbility DefendModeAbility;

        public EAOnDefendModeAbilityRemoved(IDefendModeAbility defendModeAbility)
        {
            DefendModeAbility = defendModeAbility;
        }
    }
}