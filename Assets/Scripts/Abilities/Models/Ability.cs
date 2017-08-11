using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities
{
    public abstract class Ability
    {
        protected IAbilityTemplate template;

        public Ability(IAbilityTemplate template, DefendingEntity attachedToDefendingEntity)
        {
            this.template = template;
        }

        public abstract string UIText();
    }
}