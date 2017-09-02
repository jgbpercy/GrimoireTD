using System;
using GrimoireTD.Creeps;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IProjectile
    {
        string Id { get; }

        IProjectileTemplate ProjectileTemplate { get; }

        Vector3 Position { get; }

        event EventHandler<EAOnDestroyProjectile> OnDestroyProjectile;

        void HitCreep(ICreep creep, float destructionDelay);

        void ModelObjectFrameUpdate();

        void GameObjectDestroyed();
    }
}