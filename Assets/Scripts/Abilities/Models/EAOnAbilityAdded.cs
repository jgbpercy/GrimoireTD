using System;

namespace GrimoireTD.Abilities
{
    public class EAOnAbilityAdded : EventArgs
    {
        public readonly IAbility Ability;

        public EAOnAbilityAdded(IAbility ability)
        {
            Ability = ability;
        }
    }
}