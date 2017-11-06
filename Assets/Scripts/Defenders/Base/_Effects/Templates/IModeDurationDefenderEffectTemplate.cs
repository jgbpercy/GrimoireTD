namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface IModeDurationDefenderEffectTemplate : IDefenderEffectTemplate
    {
        float BaseDuration { get; }
    }
}