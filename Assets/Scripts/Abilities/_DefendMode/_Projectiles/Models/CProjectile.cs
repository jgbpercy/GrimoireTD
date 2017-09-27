using System;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.DefendingEntities;
using GrimoireTD.Technical;
using GrimoireTD.ChannelDebug;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class CProjectile : IProjectile, IFrameUpdatee
    {
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

        public CProjectile(Vector3 startPosition, IDefendModeTargetable target, IProjectileTemplate template, IDefendingEntity sourceDefendingEntity)
        {
            id = IdGen.GetNextId();

            Position = startPosition;
            this.target = target;
            target.OnDied += OnTargetDied;
            this.sourceDefendingEntity = sourceDefendingEntity;

            ProjectileTemplate = template;

            ModelObjectFrameUpdater.Instance.RegisterAsModelObjectFrameUpdatee(this);

            CDebug.Log(CDebug.combatLog, 
                "Projectile " + Id + 
                " was created, target: " + target.Id + 
                " (" + target.NameInGame + ")");
        }

        public virtual void ModelObjectFrameUpdate()
        {
            if (destroyingForHitTarget)
            {
                return;
            }

            if (target == null)
            {
                if (!destroyingForNoTarget)
                {
                    Destroy(5f);
                    destroyingForNoTarget = true;
                }

                if (currentDirection == Vector3.zero)
                {
                    Destroy();
                }
                Position = Position + currentDirection * ProjectileTemplate.Speed * Time.deltaTime;
            }
            else
            {
                Position = Vector3.MoveTowards(Position, target.TargetPosition(), ProjectileTemplate.Speed * Time.deltaTime);
                currentDirection = (target.TargetPosition() - Position).normalized;
            }
        }

        public void HitCreep(ICreep creep, float destructionDelay)
        {
            CDebug.Log(CDebug.combatLog, 
                "Projectile " + Id + 
                " hit " + creep.Id + 
                " (" + creep.NameInGame + ")");

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

        public void GameObjectDestroyed()
        {
            ModelObjectFrameUpdater.Instance.DeregisterAsModelObjectFrameUpdatee(this);

            if (target != null)
            {
                target.OnDied -= OnTargetDied;
            }
        }
    }
}