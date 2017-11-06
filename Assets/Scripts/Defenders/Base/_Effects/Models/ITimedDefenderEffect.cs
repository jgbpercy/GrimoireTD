namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface ITimedDefenderEffect : IDefenderEffect
    {
        ITimedDefenderEffectTemplate TimedDefenderEffectTemplate { get; }

        float Duration { get; }
    }
}