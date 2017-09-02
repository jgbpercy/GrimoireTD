using System;
using System.Collections.Generic;

namespace GrimoireTD.TemporaryEffects
{
    public interface IReadOnlyTemporaryEffectsManager
    {
        IReadOnlyList<IReadOnlyTemporaryEffect> EffectList { get; }

        event EventHandler<EAOnNewTemporaryEffect> OnNewTemporaryEffect;
    }
}