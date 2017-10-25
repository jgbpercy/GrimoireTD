using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public interface IWaveTemplate
    {
        IReadOnlyList<ISpawn> Spawns { get; }
        
        IWave GenerateWave();
    }
}