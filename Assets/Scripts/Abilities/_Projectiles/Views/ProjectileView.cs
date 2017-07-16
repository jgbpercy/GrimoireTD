using UnityEngine;

public class ProjectileView : SingletonMonobehaviour<ProjectileView> {

    [SerializeField]
    private Transform projectileFolder;

    public void CreateProjectile(Projectile projectileModel)
    {
        ProjectileComponent projectileController = Instantiate(projectileModel.ProjectileClassTemplate.ProjectilePrefab, projectileModel.Position, Quaternion.identity, projectileFolder).GetComponent<ProjectileComponent>();

        projectileController.SetUp(projectileModel);
    }
}
