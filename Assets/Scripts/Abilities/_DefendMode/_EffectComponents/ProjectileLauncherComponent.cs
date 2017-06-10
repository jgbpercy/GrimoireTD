using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileLauncher", menuName = "Abilities/Effect Components/Projectile Launcher")]
public class ProjectileLauncherComponent : EffectComponent {

    [SerializeField]
    protected ProjectileTemplate projectileToFireTemplate;

    public ProjectileTemplate ProjectileToFireTemplate
    {
        get
        {
            return projectileToFireTemplate;
        }
    }

    public override void ExecuteEffect(Vector3 position, List<ITargetable> targets)
    {
        foreach (ITargetable target in targets)
        {
            projectileToFireTemplate.GenerateProjectile(position, target);
        }
    }
}
