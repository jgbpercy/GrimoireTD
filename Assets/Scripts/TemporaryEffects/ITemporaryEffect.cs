using GrimoireTD.Technical;

namespace GrimoireTD.TemporaryEffects
{
    public interface ITemporaryEffect : IReadOnlyTemporaryEffect, IFrameUpdatee
    {
        object Key { get; }

        void EndNow();
    }
}