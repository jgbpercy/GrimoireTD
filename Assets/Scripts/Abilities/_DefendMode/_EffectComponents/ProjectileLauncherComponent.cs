using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileLauncher", menuName = "Defend Mode Abilities/Effect Components/Projectile Launcher")]
public class ProjectileLauncherComponent : DMEffectComponent {

    [SerializeField]
    protected ProjectileTemplate projectileToFireTemplate;

    public ProjectileTemplate ProjectileToFireTemplate
    {
        get
        {
            return projectileToFireTemplate;
        }
    }

    public override void ExecuteEffect(DefendingEntity attachedToDefendingEntity, List<IDefendModeTargetable> targets)
    {
        foreach (IDefendModeTargetable target in targets)
        {
            projectileToFireTemplate.GenerateProjectile(attachedToDefendingEntity.CoordPosition.ToFirePointVector(), target, attachedToDefendingEntity);
        }
    }
}
