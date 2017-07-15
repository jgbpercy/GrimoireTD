using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAoeProjectile", menuName = "Defend Mode Abilities/Projectiles/AOE Projectile")]
public class AoeProjectileTemplate : ProjectileTemplate {

    [SerializeField]
    protected AttackEffect[] aoeAttackEffects;

    [SerializeField]
    protected float aoeRadius;

    [SerializeField]
    protected float aoeExpansionLerpFactor = 0.15f;

    public AttackEffect[] AoeAttackEffects
    {
        get
        {
            return aoeAttackEffects;
        }
    }

    public float AoeRadius
    {
        get
        {
            return aoeRadius;
        }
    }

    public float AoeExpansionLerpFactor
    {
        get
        {
            return aoeExpansionLerpFactor;
        }
    }

    public override Projectile GenerateProjectile(Vector3 startPosition, IDefendModeTargetable target, DefendingEntity sourceDefendingEntity)
    {
        return new AoeProjectile(startPosition, target, this, sourceDefendingEntity);
    }
}
