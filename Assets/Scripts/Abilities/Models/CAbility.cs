namespace GrimoireTD.Abilities
{
    public abstract class CAbility : IAbility
    {
        public IAbilityTemplate AbilityTemplate { get; }

        public CAbility(IAbilityTemplate abilityTemplate)
        {
            AbilityTemplate = abilityTemplate;
        }

        public abstract string UIText();
    }
}