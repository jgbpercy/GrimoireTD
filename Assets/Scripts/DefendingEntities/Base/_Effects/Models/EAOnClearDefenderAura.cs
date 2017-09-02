using System;

namespace GrimoireTD.DefendingEntities.DefenderEffects
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