using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public interface IWave
    {
        IReadOnlyDictionary<float, ICreepTemplate> Spawns { get; }

        ICreepTemplate DequeueNextCreep();

        float NextSpawnTime();

        bool CreepsRemaining();
    }
}