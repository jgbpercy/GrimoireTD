using UnityEngine;
using System;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class AoeProjectile : Projectile
    {
        private IAoeProjectileTemplate aoeProjectileTemplate;

        private float currentAoeRadius;

        private bool isExploding = false;

        protected Action OnExplosionCallback;
        protected Action OnExplosionFinishedCallback;

        public IAoeProjectileTemplate AoeProjectileClassTemplate
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

        public AoeProjectile(Vector3 startPosition, IDefendModeTargetable target, IAoeProjectileTemplate template, DefendingEntity sourceDefendingEntity) : base(startPosition, target, template, sourceDefendingEntity)
        {
            aoeProjectileTemplate = template;
            currentAoeRadius = 0.001f;
        }

        public override void ModelObjectFrameUpdate()
        {
            base.ModelObjectFrameUpdate();

            if (destroyingForHitTarget)
            {
                if (!isExploding)
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
            creep.ApplyAttackEffects(aoeProjectileTemplate.AoeAttackEffects, sourceDefendingEntity);
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
}