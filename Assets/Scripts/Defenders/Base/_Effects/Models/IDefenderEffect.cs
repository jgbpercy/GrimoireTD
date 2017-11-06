namespace GrimoireTD.Defenders.DefenderEffects
{
    public interface IDefenderEffect
    {
        IDefenderEffectTemplate DefenderEffectTemplate { get; }

        string UIText();
    }
}