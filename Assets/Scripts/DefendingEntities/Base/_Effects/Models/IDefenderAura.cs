using System;

namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface IDefenderAura : IDefenderEffect
    {
        IDefenderAuraTemplate DefenderAuraTemplate { get; }

        int Range { get; }

        IDefendingEntity SourceDefendingEntity { get; }

        event EventHandler<EAOnClearDefenderAura> OnClearDefenderAura;

        void ClearAura();
    }
}