using System;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IAoeProjectile : IProjectile
    {
        IAoeProjectileTemplate AoeProjectileTemplate { get; }

        float CurrentAoeRadius { get; }

        event EventHandler<EAOnExplosionStarted> OnExplosionStarted;
        event EventHandler<EAOnExplosionFinished> OnExplosionFinished;

        void HitCreepInAoe(ICreep creep);
    }
}