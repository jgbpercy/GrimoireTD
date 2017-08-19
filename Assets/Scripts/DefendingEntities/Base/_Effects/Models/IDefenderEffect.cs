namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public interface IDefenderEffect
    {
        IDefenderEffectTemplate DefenderEffectTemplate { get; }

        string UIText();
    }
}