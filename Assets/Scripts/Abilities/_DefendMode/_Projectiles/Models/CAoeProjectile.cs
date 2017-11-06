using UnityEngine;
using System;
using GrimoireTD.Creeps;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class CAoeProjectile : CProjectile, IAoeProjectile
    {
        public IAoeProjectileTemplate AoeProjectileTemplate { get; }

        public float CurrentAoeRadius { get; private set; }

        private bool isExploding;
        private float timeExploding;

        public event EventHandler<EAOnExplosionStarted> OnExplosionStarted;
        public event EventHandler<EAOnExplosionFinished> OnExplosionFinished;

        public CAoeProjectile(
            Vector3 startPosition, 
            IDefendModeTargetable target, 
            IAoeProjectileTemplate template, 
            IDefender sourceDefender
        ) : base(startPosition, target, template, sourceDefender)
        {
            AoeProjectileTemplate = template;
            CurrentAoeRadius = 0.0000001f;
            timeExploding = 0f;
            isExploding = false;
        }

        protected override void ModelObjectFrameUpdate(float deltaTime)
        {
            base.ModelObjectFrameUpdate(deltaTime);

            if (destroyingForHitTarget)
            {
                if (!isExploding)
                {
                    isExploding = true;
                    OnExplosionStarted?.Invoke(this, new EAOnExplosionStarted());
                }

                timeExploding += deltaTime;

                CurrentAoeRadius = Mathf.Lerp(
                    0f, 
                    AoeProjectileTemplate.AoeRadius, 
                    Mathf.Pow(timeExploding/AoeProjectileTemplate.AoeExplosionTime, 1/3)
                );
                
                if (AoeProjectileTemplate.AoeExplosionTime - timeExploding < 0.01f)
                {
                    OnExplosionFinished?.Invoke(this, new EAOnExplosionFinished());
                }
            }
        }

        public virtual void HitCreepInAoe(ICreep creep)
        {
            creep.ApplyAttackEffects(AoeProjectileTemplate.AoeAttackEffects, sourceDefender);
        }
    }
}