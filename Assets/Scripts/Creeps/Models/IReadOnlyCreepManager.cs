using System;
using UnityEngine;
using GrimoireTD.Abilities.DefendMode;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyCreepManager
    {
        IDefendModeTargetable CreepInRangeNearestToEnd(Vector3 fromPosition, float range);

        event EventHandler<EAOnWaveOver> OnWaveOver;

        event EventHandler<EAOnCreepSpawned> OnCreepSpawned;
    }
}