using System;
using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.DefendingEntities
{
    public class EAOnProjectileCreated : EventArgs
    {
        public readonly IProjectile Projectile;

        public EAOnProjectileCreated(IProjectile projectile)
        {
            Projectile = projectile;
        }
    }
}