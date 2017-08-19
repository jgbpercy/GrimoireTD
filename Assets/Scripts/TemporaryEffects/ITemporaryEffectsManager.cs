using System;

namespace GrimoireTD.TemporaryEffects
{
    public interface ITemporaryEffectsManager : IReadOnlyTemporaryEffectsManager
    {
        void ApplyEffect(object key, float magnitude, float duration, string effectName, Action onApplyCallback, Action onEndCallback);
    }
}