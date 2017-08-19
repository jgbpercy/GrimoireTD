using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.AttackEffects;

namespace GrimoireTD.Creeps
{
    [Serializable]
    public class SBlockModifier : IBlockModifier
    {
        [SerializeField]
        private SoSpecificDamageEffectType damageType;

        [SerializeField]
        private int magnitude;

        public ISpecificDamageEffectType DamageType
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
    }
}