using System;

namespace GrimoireTD.Abilities
{
    public abstract class CAbility : IAbility
    {
        public IAbilityTemplate AbilityTemplate { get; }

        public CAbility(IAbilityTemplate abilityTemplate)
        {
            AbilityTemplate = abilityTemplate;
        }

        public event EventHandler<EAOnAbilityExecuted> OnAbilityExecuted;

        protected virtual void OnAbilityExecutedVirtual(EAOnAbilityExecuted args)
        {
            EventHandler<EAOnAbilityExecuted> tempCopy = OnAbilityExecuted;

            tempCopy?.Invoke(this, args);
        }

        public abstract string UIText();
    }
}