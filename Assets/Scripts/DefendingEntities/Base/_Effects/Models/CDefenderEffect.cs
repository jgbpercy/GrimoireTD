namespace GrimoireTD.DefendingEntities.DefenderEffects
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