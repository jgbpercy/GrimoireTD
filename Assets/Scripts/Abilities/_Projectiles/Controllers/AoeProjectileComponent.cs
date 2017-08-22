using UnityEngine;
using UnityEngine.Assertions;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class AoeProjectileComponent : ProjectileComponent
    {
        private bool isExploding = false;

        private IAoeProjectile aoeProjectileModel;

        public override void SetUp(IProjectile projectileModel)
        {
            base.SetUp(projectileModel);

            aoeProjectileModel = projectileModel as IAoeProjectile;
            Assert.IsNotNull(aoeProjectileModel);

            aoeProjectileModel.RegisterForOnExplosionCallback(() => { isExploding = true; });
            aoeProjectileModel.RegisterForOnExplosionFinishedCallback(OnExplosionFinished);
        }

        private void OnExplosionFinished()
        {
            if (ownCollider != null)
            {
                ownCollider.enabled = false;
            }

            isExploding = false;
        }

        //TODO handle this in the model where it should be?
        protected override void OnTriggerEnter(Collider other)
        {
            if (!isExploding)
            {
                base.OnTriggerEnter(other);
                ownCollider.enabled = true;
            }
            else
            {
                if (other.CompareTag("Creep"))
                {
                    ICreep hitCreep = other.GetComponent<CreepComponent>().CreepModel;
                    aoeProjectileModel.HitCreepInAoe(hitCreep);
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (isExploding)
            {
                ownCollider.radius = aoeProjectileModel.CurrentAoeRadius;
            }
        }
    }
}