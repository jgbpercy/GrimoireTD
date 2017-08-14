using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class SBlockModifier : IBlockModifier
    {
        [SerializeField]
        private SpecificDamageEffectType damageType;

        [SerializeField]
        private int magnitude;

        public SpecificDamageEffectType DamageType
        {
            get
            {
                return damageType;
            }
        }

        public int Magnitude
        {
            get
            {
                return magnitude;
            }
        }

        public SBlockModifier(int magnitude, SpecificDamageEffectType damageType)
        {
            this.magnitude = magnitude;
            this.damageType = damageType;
        }
    }
}