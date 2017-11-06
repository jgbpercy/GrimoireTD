using System.Collections.Generic;
using GrimoireTD.Abilities.DefendMode.Projectiles;
using GrimoireTD.Defenders;

namespace GrimoireTD.Abilities.DefendMode
{
    public class CProjectileLauncherComponent : IProjectileLauncherComponent
    {
        private IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate;

        public CProjectileLauncherComponent(IProjectileLauncherComponentTemplate projectileLauncherComponentTemplate)
        {
            this.projectileLauncherComponentTemplate = projectileLauncherComponentTemplate;
        }

        public void ExecuteEffect(IDefender attachedToDefender, IReadOnlyList<IDefendModeTargetable> targets)
        {
            foreach (IDefendModeTargetable target in targets)
            {
                IProjectile projectile = projectileLauncherComponentTemplate.ProjectileToFireTemplate.GenerateProjectile
                (
                    attachedToDefender.CoordPosition.ToFirePointVector(),
                    target,
                    attachedToDefender
                );

                //TODO: is there a good way to make this an event?
                attachedToDefender.CreatedProjectile(projectile);
            }
        }
    }
}