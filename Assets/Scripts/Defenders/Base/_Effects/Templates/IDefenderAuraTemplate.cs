namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface IDefenderAuraTemplate : IDefenderEffectTemplate
    {
        int BaseRange { get; }

        bool AffectsSelf { get; }

        IDefenderAura GenerateDefenderAura(IDefender sourceDefender);
    }
}