using System.Collections.Generic;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    [CreateAssetMenu(fileName = "NewProjectileLauncher", menuName = "Defend Mode Abilities/Effect Components/Projectile Launcher")]
    public class SoProjectileLauncherComponent : SoDefendModeEffectComponent, IProjectileLauncherComponent
    {
        [SerializeField]
        protected SoProjectileTemplate projectileToFireTemplate;

        public IProjectileTemplate ProjectileToFireTemplate
        {
            get
            {
                return projectileToFireTemplate;
            }
        }

        public override void ExecuteEffect(IDefendingEntity attachedToDefendingEntity, IReadOnlyList<IDefendModeTargetable> targets)
        {
            foreach (IDefendModeTargetable target in targets)
            {
                IProjectile projectile = projectileToFireTemplate.GenerateProjectile
                (
                    attachedToDefendingEntity.CoordPosition.ToFirePointVector(), 
                    target, 
                    attachedToDefendingEntity
                );

                attachedToDefendingEntity.CreatedProjectile(projectile);
            }
        }
    }
}