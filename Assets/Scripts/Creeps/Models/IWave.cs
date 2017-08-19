using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public interface IWave
    {
        IReadOnlyDictionary<float, ICreepTemplate> Spawns { get; }

        ICreepTemplate SpawnNextCreep();

        float NextSpawnTime();
    }
}