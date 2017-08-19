using System;
using UnityEngine;
using GrimoireTD.DefendingEntities;
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

        public float GetActualMagnitude(IDefendingEntity sourceDefendingEntity)
        {
            IDamageEffectType damageEffectType = attackEffectType as IDamageEffectType;
            if (damageEffectType != null)
            {
                return GetDamage(sourceDefendingEntity, damageEffectType);
            }

            return baseMagnitude;
        }

        private float GetDamage(IDefendingEntity sourceDefendingEntity, IDamageEffectType damageEffectType)
        {
            return (1 + sourceDefendingEntity.Attributes.GetAttribute(DefendingEntityAttributeName.damageBonus)) * baseMagnitude;
        }

        public float GetActualDuration(IDefendingEntity sourceDefendingEntity)
        {
            return BaseDuration;
        }
    }
}