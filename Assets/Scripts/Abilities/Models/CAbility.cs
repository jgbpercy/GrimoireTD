namespace GrimoireTD.Abilities
{
    public abstract class CAbility : IAbility
    {
        protected IAbilityTemplate template;

        public IAbilityTemplate Template
        {
            get
            {
                return template;
            }
        }

        public CAbility(IAbilityTemplate template)
        {
            this.template = template;
        }

        public abstract string UIText();
    }
}