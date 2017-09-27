using System;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public class EAOnDestroyProjectile : EventArgs
    {
        public readonly float WaitSeconds;

        public EAOnDestroyProjectile(float waitSeconds)
        {
            WaitSeconds = waitSeconds;
        }
    }
}