using System;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public class EAOnAnyBlockChanged : EventArgs
    {
        public readonly ISpecificDamageEffectType DamageType;

        public readonly int NewValue;

        public EAOnAnyBlockChanged(ISpecificDamageEffectType damageType, int newValue)
        {
            DamageType = damageType;
            NewValue = newValue;
        }
    }
}