using System;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.Dependencies;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class CProjectile : IProjectile
    {
        public const float NoTargetDestructionDelay = 5f;

        protected int id;

        public IProjectileTemplate ProjectileTemplate { get; }

        public Vector3 Position { get; private set; }
        private Vector3 currentDirection;

        protected IDefendingEntity sourceDefendingEntity;

        private IDefendModeTargetable target;

        protected bool destroyingForHitTarget;
        private bool destroyingForNoTarget;

        public event EventHandler<EAOnDestroyProjectile> OnDestroyProjectile;

        public string Id
        {
            get
            {
                return "P-" + id;
            }
        }

        public CProjectile(
            Vector3 startPosition, 
            IDefendModeTargetable target, 
            IProjectileTemplate template, 
            IDefendingEntity sourceDefendingEntity
        )
        {
            id = IdGen.GetNextId();

            Position = startPosition;

            this.target = target;
            target.OnDied += OnTargetDied;

            this.sourceDefendingEntity = sourceDefendingEntity;

            ProjectileTemplate = template;

            DependencyProvider.TheModelObjectFrameUpdater().Register(ModelObjectFrameUpdate);
        }

        protected virtual void ModelObjectFrameUpdate(float deltaTime)
        {
            if (destroyingForHitTarget)
            {
                return;
            }

            if (target == null)
            {
                if (!destroyingForNoTarget)
                {
                    Destroy(NoTargetDestructionDelay);
                    destroyingForNoTarget = true;
                }

                if (currentDirection == Vector3.zero)
                {
                    Destroy();
                }
                
                Position = Position + currentDirection * ProjectileTemplate.Speed * deltaTime;
            }
            else
            {
                Position = Vector3.MoveTowards(Position, target.TargetPosition(), ProjectileTemplate.Speed * deltaTime);
                currentDirection = (target.TargetPosition() - Position).normalized;
            }
        }

        public void HitCreep(ICreep creep, float destructionDelay)
        { 
            //TODO: apply modifiers from defending entity at the point of projectile creation, rather than at the point of effect application?
            creep.ApplyAttackEffects(ProjectileTemplate.AttackEffects, sourceDefendingEntity);
            destroyingForHitTarget = true;
            Destroy(destructionDelay);
        }

        private void OnTargetDied(object sender, EventArgs args)
        {
            target = null;
        }

        private void Destroy()
        {
            OnDestroyProjectile?.Invoke(this, new EAOnDestroyProjectile(0f));
        }

        private void Destroy(float waitSeconds)
        {
            OnDestroyProjectile?.Invoke(this, new EAOnDestroyProjectile(waitSeconds));
        }

        //TODO get rid of this
        public void GameObjectDestroyed()
        {
            DependencyProvider.TheModelObjectFrameUpdater().Deregister(ModelObjectFrameUpdate);

            if (target != null)
            {
                target.OnDied -= OnTargetDied;
            }
        }
    }
}