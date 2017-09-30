using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.DefendingEntities;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CProjectileLauncherComponent : IProjectileLauncherComponent
    {
        private IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate;

        public CProjectileLauncherComponent(IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate)
        {
            this.projectileLauncherComponentTemplate = projectileLauncherComponentTemplate;
        }

        public void ExecuteEffect(IDefendingEntity attachedToDefendingEntity, IReadOnlyList<IDefendModeTargetable> targets)
        {
            foreach (IDefendModeTargetable target in targets)
            {
                IProjectile projectile = projectileLauncherComponentTemplate.ProjectileToFireTemplate.GenerateProjectile
                (
                    attachedToDefendingEntity.CoordPosition.ToFirePointVector(),
                    target,
                    attachedToDefendingEntity
                );

                //TODO: is there a good way to make this an event?
                attachedToDefendingEntity.CreatedProjectile(projectile);
            }
        }
    }
}