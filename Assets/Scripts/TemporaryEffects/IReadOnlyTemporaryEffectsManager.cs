using System;
using System.Collections.Generic;

namespace GrimoireTD.TemporaryEffects
{
    public interface IReadOnlyTemporaryEffectsManager
    {
        IReadOnlyList<IReadOnlyTemporaryEffect> EffectList { get; }

        void RegisterForOnNewTemporaryEffectCallback(Action<IReadOnlyTemporaryEffect> callback);

        void DeregisterForOnNewTemporaryEffectCallback(Action<IReadOnlyTemporaryEffect> callback);
    }
}