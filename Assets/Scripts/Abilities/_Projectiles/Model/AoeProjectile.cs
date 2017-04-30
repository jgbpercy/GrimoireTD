using UnityEngine;
using System;

public class AoeProjectile : Projectile {

    private AoeProjectileTemplate aoeProjectileTemplate;

    private float currentAoeRadius;

    private bool isExploding = false;

    protected Action OnExplosionCallback;
    protected Action OnExplosionFinishedCallback;

    public AoeProjectileTemplate AoeProjectileClassTemplate
    {
        get
        {
            return aoeProjectileTemplate;
        }
    }

    public float CurrentAoeRadius
    {
        get
        {
            return currentAoeRadius;
        }
    }

    public AoeProjectile(Vector3 startPosition, ITargetable target, AoeProjectileTemplate template) : base(startPosition, target, template)
    {
        aoeProjectileTemplate = template;
        currentAoeRadius = 0.001f;
    }

    public override void ModelObjectFrameUpdate()
    {
        base.ModelObjectFrameUpdate();

        if (destroyingForHitTarget)
        {
            if ( !isExploding )
            {
                isExploding = true;
                OnExplosionCallback();
            }

            currentAoeRadius = Mathf.Lerp(currentAoeRadius, aoeProjectileTemplate.AoeRadius, aoeProjectileTemplate.AoeExpansionLerpFactor);

            if (aoeProjectileTemplate.AoeRadius - currentAoeRadius < 0.01f)
            {
                OnExplosionFinishedCallback();
            }
        }
    }

    public virtual void HitCreepInAoe(Creep creep)
    {
        creep.ApplyAttackEffects(aoeProjectileTemplate.AoeAttackEffects);
    }

    public void RegisterForOnExplosionCallback(Action callback)
    {
        OnExplosionCallback += callback;
    }

    public void DeregisterForOnExplosionCallback(Action callback)
    {
        OnExplosionCallback -= callback;
    }

    public void RegisterForOnExplosionFinishedCallback(Action callback)
    {
        OnExplosionFinishedCallback += callback;
    }

    public void DeregisterForOnExplosionFinishedCallback(Action callback)
    {
        OnExplosionFinishedCallback -= callback;
    }
}
