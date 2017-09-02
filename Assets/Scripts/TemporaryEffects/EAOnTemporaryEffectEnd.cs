using System;

namespace GrimoireTD.TemporaryEffects
{
    public class EAOnTemporaryEffectEnd : EventArgs
    {
        public readonly ITemporaryEffect EndedEffect;

        public EAOnTemporaryEffectEnd(ITemporaryEffect endedEffect)
        {
            EndedEffect = endedEffect;
        }
    }
}