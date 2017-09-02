using System;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    public class EAOnAnyResistanceChanged : EventArgs
    {
        public readonly ISpecificDamageEffectType DamageType;

        public readonly float NewValue;

        public EAOnAnyResistanceChanged(ISpecificDamageEffectType damageType, float newValue)
        {
            DamageType = damageType;
            NewValue = newValue;
        }
    }
}