using System;

namespace GrimoireTD.Abilities
{
    public class EAOnAbilityRemoved : EventArgs
    {
        public readonly IAbility Ability;

        public EAOnAbilityRemoved(IAbility ability)
        {
            Ability = ability;
        }
    }
}