using System;

namespace GrimoireTD.TemporaryEffects
{
    public interface IReadOnlyTemporaryEffect
    {
        float Magnitude { get; }

        float Duration { get; }
        float Elapsed { get; }

        float TimeRemaining { get; }

        string EffectName { get; }

        event EventHandler<EAOnTemporaryEffectEnd> OnTemporaryEffectEnd;
    }
}