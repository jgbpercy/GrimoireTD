using System;
using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public interface IReadOnlyCreepManager
    {
        IReadOnlyList<ICreep> CreepList { get; }

        event EventHandler<EAOnWaveOver> OnWaveOver;

        event EventHandler<EAOnCreepSpawned> OnCreepSpawned;
    }
}