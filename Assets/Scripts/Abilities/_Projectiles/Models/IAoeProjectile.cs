using System;
using GrimoireTD.Creeps;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IAoeProjectile
    {
        IAoeProjectileTemplate AoeProjectileClassTemplate { get; }

        float CurrentAoeRadius { get; }

        void HitCreepInAoe(ICreep creep);

        void ModelObjectFrameUpdate();

        void RegisterForOnExplosionCallback(Action callback);

        void DeregisterForOnExplosionCallback(Action callback);

        void RegisterForOnExplosionFinishedCallback(Action callback);

        void DeregisterForOnExplosionFinishedCallback(Action callback);
    }
}