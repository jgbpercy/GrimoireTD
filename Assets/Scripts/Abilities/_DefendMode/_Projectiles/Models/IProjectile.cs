using System;
using UnityEngine;
using GrimoireTD.Creeps;
using GrimoireTD.Technical;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IProjectile : IFrameUpdatee
    {
        string Id { get; }

        IProjectileTemplate ProjectileTemplate { get; }

        Vector3 Position { get; }

        event EventHandler<EAOnDestroyProjectile> OnDestroyProjectile;

        void HitCreep(ICreep creep, float destructionDelay);

        void GameObjectDestroyed();
    }
}