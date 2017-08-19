using UnityEngine;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class ProjectileView : SingletonMonobehaviour<ProjectileView>
    {
        [SerializeField]
        private Transform projectileFolder;

        public void CreateProjectile(IProjectile projectileModel)
        {
            ProjectileComponent projectileController = Instantiate(projectileModel.ProjectileClassTemplate.ProjectilePrefab, projectileModel.Position, Quaternion.identity, projectileFolder).GetComponent<ProjectileComponent>();

            projectileController.SetUp(projectileModel);
        }
    }
}