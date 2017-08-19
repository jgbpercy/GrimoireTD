using System;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyResistances
    {
        float GetResistance(IDamageEffectType damageEffectType, float currentArmor);

        int GetBlock(IDamageEffectType damageEffectType);

        void RegisterForOnResistanceChangedCallback(Action<float> callback, ISpecificDamageEffectType specificDamageEffectType);
        void DeregisterForOnResistanceChangedCallback(Action<float> callback, ISpecificDamageEffectType specificDamageEffectType);

        void RegisterForOnAnyResistanceChangedCallback(Action<ISpecificDamageEffectType, float> callback);
        void DeregisterForOnAnyResistanceChangedCallback(Action<ISpecificDamageEffectType, float> callback);

        void RegisterForOnBlockChanged(Action<int> callback, ISpecificDamageEffectType specificDamageEffectType);
        void DeregisterForOnBlockChanged(Action<int> callback, ISpecificDamageEffectType specificDamageEffectType);

        void RegisterForOnAnyBlockChangedCallback(Action<ISpecificDamageEffectType, int> callback);
        void DeregisterForOnAnyBlockChangeCallback(Action<ISpecificDamageEffectType, int> callback);
    }
}