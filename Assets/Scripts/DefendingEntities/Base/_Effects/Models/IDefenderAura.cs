using System;

namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface IDefenderAura : IDefenderEffect
    {
        IDefenderAuraTemplate DefenderAuraTemplate { get; }

        int Range { get; }

        IDefendingEntity SourceDefendingEntity { get; }

        void ClearAura();

        void DeregisterForOnClearAuraCallback(Action<IDefenderAura> callback);
        void RegisterForOnClearAuraCallback(Action<IDefenderAura> callback);
    }
}