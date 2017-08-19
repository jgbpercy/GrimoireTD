namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface IDefenderAuraTemplate : IDefenderEffectTemplate
    {
        int BaseRange { get; }

        bool AffectsSelf { get; }

        IDefenderAura GenerateDefenderAura(IDefendingEntity sourceDefendingEntity);
    }
}