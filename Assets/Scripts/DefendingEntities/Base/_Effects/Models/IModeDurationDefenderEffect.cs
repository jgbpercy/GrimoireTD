namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface IModeDurationDefenderEffect : IDefenderEffect
    {
        ModeDurationDefenderEffectTemplate ModeDurationDefenderEffectTemplate { get; }

        int Duration { get; }
    }
}