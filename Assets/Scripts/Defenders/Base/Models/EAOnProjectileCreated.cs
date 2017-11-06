using System;
using GrimoireTD.Abilities.DefendMode.Projectiles;

namespace GrimoireTD.Defenders
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