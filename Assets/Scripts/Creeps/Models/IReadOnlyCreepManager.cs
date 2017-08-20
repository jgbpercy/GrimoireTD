using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyCreepManager
    {
        IDefendModeTargetable CreepInRangeNearestToEnd(Vector3 fromPosition, float range);

        void RegisterForOnWaveIsOverCallback(Action callback);
        void DeregisterForOnWaveIsOverCallback(Action callback);

        void RegisterForCreepSpawnedCallback(Action<ICreep> callback);
        void DeregisterForCreepSpawnedCallback(Action<ICreep> callback);
    }
}