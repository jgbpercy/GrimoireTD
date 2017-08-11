namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface ITimedDefenderEffectTemplate : IDefenderEffectTemplate
    {
        int BaseDuration { get; }
    }
}