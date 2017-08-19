using System;
using GrimoireTD.Creeps;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.Projectiles
{
    public interface IProjectile
    {
        string Id { get; }

        IProjectileTemplate ProjectileClassTemplate { get; }

        Vector3 Position { get; }

        void HitCreep(ICreep creep, float destructionDelay);

        void ModelObjectFrameUpdate();

        void GameObjectDestroyed();

        void RegisterForOnDestroyCallback(Action<float> callback);

        void DeregisterForOnDestroyCallback(Action<float> callback);
    }
}