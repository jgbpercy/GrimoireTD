namespace GrimoireTD.Defenders.DefenderEffects
{
    public abstract class CDefenderEffect : IDefenderEffect
    {
        public IDefenderEffectTemplate DefenderEffectTemplate { get; }

        public CDefenderEffect(IDefenderEffectTemplate defenderEffectTemplate)
        {
            DefenderEffectTemplate = defenderEffectTemplate;
        }

        public abstract string UIText();
    }
}