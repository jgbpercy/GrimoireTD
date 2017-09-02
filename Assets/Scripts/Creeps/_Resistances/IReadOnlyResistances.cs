using System;
using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyResistances
    {
        IReadOnlyRecalculatorList<float> GetBaseResistance(IDamageEffectType damageEffectType);

        IReadOnlyRecalculatorList<int> GetBlock(IDamageEffectType damageEffectType);

        float GetResistanceAfterArmor(IDamageEffectType damageEffectType, float currentArmor);

        event EventHandler<EAOnAnyResistanceChanged> OnAnyResistanceChanged;

        event EventHandler<EAOnAnyBlockChanged> OnAnyBlockChanged;
    }
}