using System;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyResistances
    {
        float GetResistance(DamageEffectType damageEffectType, float currentArmor);

        int GetBlock(DamageEffectType damageEffectType);

        void RegisterForOnResistanceChangedCallback(Action<float> callback, SpecificDamageEffectType specificDamageEffectType);
        void DeregisterForOnResistanceChangedCallback(Action<float> callback, SpecificDamageEffectType specificDamageEffectType);

        void RegisterForOnAnyResistanceChangedCallback(Action<SpecificDamageEffectType, float> callback);
        void DeregisterForOnAnyResistanceChangedCallback(Action<SpecificDamageEffectType, float> callback);

        void RegisterForOnBlockChanged(Action<int> callback, SpecificDamageEffectType specificDamageEffectType);
        void DeregisterForOnBlockChanged(Action<int> callback, SpecificDamageEffectType specificDamageEffectType);

        void RegisterForOnAnyBlockChangedCallback(Action<SpecificDamageEffectType, int> callback);
        void DeregisterForOnAnyBlockChangeCallback(Action<SpecificDamageEffectType, int> callback);
    }
}