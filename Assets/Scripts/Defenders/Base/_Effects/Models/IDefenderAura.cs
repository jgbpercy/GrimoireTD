using System;

namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface IDefenderAura : IDefenderEffect
    {
        IDefenderAuraTemplate DefenderAuraTemplate { get; }

        int Range { get; }

        IDefender SourceDefender { get; }

        event EventHandler<EAOnClearDefenderAura> OnClearDefenderAura;

        void ClearAura();
    }
}