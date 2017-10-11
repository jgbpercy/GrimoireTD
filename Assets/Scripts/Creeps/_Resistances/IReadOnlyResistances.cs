using System;
using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyResistances
    {
        IReadOnlyRecalculatorList<float> GetResistance(IDamageEffectType damageEffectType);

        IReadOnlyRecalculatorList<int> GetBlock(IDamageEffectType damageEffectType);

        float GetResistanceWithoutArmor(IDamageEffectType damageEffectType);

        event EventHandler<EAOnAnyResistanceChanged> OnAnyResistanceChanged;

        event EventHandler<EAOnAnyBlockChanged> OnAnyBlockChanged;
    }
}