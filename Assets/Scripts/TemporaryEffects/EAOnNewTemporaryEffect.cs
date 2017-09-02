using System;

namespace GrimoireTD.TemporaryEffects
{
    public class EAOnNewTemporaryEffect : EventArgs
    {
        public readonly IReadOnlyTemporaryEffect NewEffect;

        public EAOnNewTemporaryEffect(IReadOnlyTemporaryEffect newEffect)
        {
            NewEffect = newEffect;
        }
    }
}