using System.Collections.Generic;

namespace GrimoireTD.Creeps
{
    public interface ICreepManager : IReadOnlyCreepManager
    {
        void SetUp(IEnumerable<IWaveTemplate> waves, float idleTimeToTrackAfterSpawnEnd);
    }
}