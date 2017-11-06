using System;
using UnityEngine;
using GrimoireTD.Defenders;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [Serializable]
    public class SAttackEffect : IAttackEffect
    {
        [SerializeField]
        private SoAttackEffectType attackEffectType;

        [SerializeField]
        private float baseMagnitude;

        [SerializeField]
        private float baseDuration;
         
        public IAttackEffectType AttackEffectType
        {
            get
            {
                return attackEffectType;
            }
        }

        public string EffectName
        {
            get
            {
                return attackEffectType.EffectName();
            }
        }

        public float BaseMagnitude
        {
            get
            {
                return baseMagnitude;
            }
        }

        public float BaseDuration
        {
            get
            {
                return baseDuration;
            }
        }

        public float GetActualMagnitude(IDefender sourceDefender)
        {
            IDamageEffectType damageEffectType = attackEffectType as IDamageEffectType;
            if (damageEffectType != null)
            {
                return GetDamage(sourceDefender, damageEffectType);
            }

            return baseMagnitude;
        }

        private float GetDamage(IDefender sourceDefender, IDamageEffectType damageEffectType)
        {
            return (1 + sourceDefender.Attributes.Get(DeAttrName.damageBonus).Value()) * baseMagnitude;
        }

        public float GetActualDuration(IDefender sourceDefender)
        {
            return BaseDuration;
        }
    }
}