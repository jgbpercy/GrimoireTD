namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface IModeDurationDefenderEffect : IDefenderEffect
    {
        ModeDurationDefenderEffectTemplate ModeDurationDefenderEffectTemplate { get; }

        int Duration { get; }
    }
}