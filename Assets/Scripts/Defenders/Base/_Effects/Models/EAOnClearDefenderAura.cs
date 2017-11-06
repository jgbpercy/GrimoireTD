using System;

namespace GrimoireTD.Defenders.DefenderEffects
{
    public class EAOnClearDefenderAura : EventArgs
    {
        public readonly IDefenderAura ClearedAura;

        public EAOnClearDefenderAura(IDefenderAura clearedAura)
        {
            ClearedAura = clearedAura;
        }
    }
}