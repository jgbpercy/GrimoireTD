using System;

namespace GrimoireTD.Abilities
{
    public interface IAbility
    {
        IAbilityTemplate AbilityTemplate { get; }

        event EventHandler<EAOnAbilityExecuted> OnAbilityExecuted;

        string UIText();
    }
}