using UnityEngine;
using System;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class CAoeProjectile : CProjectile, IAoeProjectile
    {
        public IAoeProjectileTemplate AoeProjectileTemplate { get; }

        public float CurrentAoeRadius { get; private set; }

        private bool isExploding;

        public event EventHandler<EAOnExplosionStarted> OnExplosionStarted;
        public event EventHandler<EAOnExplosionFinished> OnExplosionFinished;

        public CAoeProjectile(Vector3 startPosition, IDefendModeTargetable target, IAoeProjectileTemplate template, IDefendingEntity sourceDefendingEntity) : base(startPosition, target, template, sourceDefendingEntity)
        {
            AoeProjectileTemplate = template;
            CurrentAoeRadius = 0.001f;
            isExploding = false;
        }

        public override void ModelObjectFrameUpdate(float deltaTime)
        {
            base.ModelObjectFrameUpdate(deltaTime);

            if (destroyingForHitTarget)
            {
                if (!isExploding)
                {
                    isExploding = true;
                    OnExplosionStarted?.Invoke(this, new EAOnExplosionStarted());
                }

                CurrentAoeRadius = Mathf.Lerp(CurrentAoeRadius, AoeProjectileTemplate.AoeRadius, AoeProjectileTemplate.AoeExpansionLerpFactor);

                if (AoeProjectileTemplate.AoeRadius - CurrentAoeRadius < 0.01f)
                {
                    OnExplosionFinished?.Invoke(this, new EAOnExplosionFinished());
                }
            }
        }

        public virtual void HitCreepInAoe(ICreep creep)
        {
            creep.ApplyAttackEffects(AoeProjectileTemplate.AoeAttackEffects, sourceDefendingEntity);
        }
    }
}