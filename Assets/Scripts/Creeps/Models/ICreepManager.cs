using System.Collections.Generic;
using GrimoireTD.Technical;

namespace GrimoireTD.Creeps
{
    public interface ICreepManager : IReadOnlyCreepManager, IFrameUpdatee
    {
        void SetUp(IEnumerable<IWaveTemplate> waves, float idleTimeToTrackAfterSpawnEnd);
    }
}