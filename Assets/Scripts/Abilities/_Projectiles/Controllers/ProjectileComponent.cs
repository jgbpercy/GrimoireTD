using UnityEngine;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class ProjectileComponent : MonoBehaviour
    {
        private IProjectile projectileModel;

        [SerializeField]
        private ParticleSystem hitExplosion;
        [SerializeField]
        private MeshRenderer ownRenderer;

        protected SphereCollider ownCollider;

        public virtual void SetUp(IProjectile projectileModel)
        {
            this.projectileModel = projectileModel;

            projectileModel.RegisterForOnDestroyCallback((waitSeconds) => { Destroy(gameObject, waitSeconds); });
        }

        private void Start()
        {
            ownCollider = gameObject.GetComponent<SphereCollider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Creep"))
            {
                ICreep hitCreep = other.GetComponent<CreepComponent>().CreepModel;
                hitExplosion.Play();
                ownRenderer.enabled = false;
                ownCollider.enabled = false;
                projectileModel.HitCreep(hitCreep, hitExplosion.main.duration - 0.01f);
            }
        }

        protected virtual void Update()
        {
            transform.position = projectileModel.Position;
        }

        private void OnDestroy()
        {
            projectileModel.GameObjectDestroyed();
        }
    }
}