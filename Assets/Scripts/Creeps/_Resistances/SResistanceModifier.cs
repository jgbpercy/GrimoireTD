using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class SResistanceModifier : IResistanceModifier
    {
        [SerializeField]
        private SoSpecificDamageEffectType damageType;

        [SerializeField]
        private float magnitude;

        public ISpecificDamageEffectType DamageType
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

        public string AsPercentageString
        {
            get
            {
                return magnitude * 100 + "%";
            }
        }
    }
}