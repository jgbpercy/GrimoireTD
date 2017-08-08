using System;
using UnityEngine;

[Serializable]
public class AttackEffect {

    [SerializeField]
    private AttackEffectType attackEffectType;

    [SerializeField]
    private int baseMagnitude;

    [SerializeField]
    private float baseDuration;

    public AttackEffectType AttackEffectType
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

    public int BaseMagnitude
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

    public float GetActualMagnitude(DefendingEntity sourceDefendingEntity)
    {
        DamageEffectType damageEffectType = attackEffectType as DamageEffectType;
        if ( damageEffectType != null )
        {
            return GetDamage(sourceDefendingEntity, damageEffectType);
        }

        return baseMagnitude;
    }
        
    private float GetDamage(DefendingEntity sourceDefendingEntity, DamageEffectType damageEffectType)
    {
        return (1 + sourceDefendingEntity.GetAttribute(AttributeName.damageBonus)) * baseMagnitude;
    }

    public float GetActualDuration(DefendingEntity sourceDefendingEntity)
    {
        return BaseDuration;
    }

}
