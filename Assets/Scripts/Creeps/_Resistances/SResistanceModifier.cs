using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class SResistanceModifier : IResistanceModifier
    {
        [SerializeField]
        private SpecificDamageEffectType damageType;

        [SerializeField]
        private float magnitude;

        public SpecificDamageEffectType DamageType
        {
            get
            {
                return damageType;
            }
        }

        public float Magnitude
        {
            get
            {
                return magnitude;
            }
        }

        public SResistanceModifier(float magnitude, SpecificDamageEffectType damageType)
        {
            this.magnitude = magnitude;
            this.damageType = damageType;
        }

        public string AsPercentageString
        {
            get
            {
                return magnitude * 100 + "%";
            }
        }
    }
}