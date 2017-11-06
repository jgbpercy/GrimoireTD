namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface ITimedDefenderEffectTemplate : IDefenderEffectTemplate
    {
        int BaseDuration { get; }
    }
}