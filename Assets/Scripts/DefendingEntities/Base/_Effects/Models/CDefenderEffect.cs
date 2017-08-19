namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public abstract class CDefenderEffect : IDefenderEffect
    {
        private IDefenderEffectTemplate defenderEffectTemplate;

        public IDefenderEffectTemplate DefenderEffectTemplate
        {
            get
            {
                return defenderEffectTemplate;
            }
        }

        public CDefenderEffect(IDefenderEffectTemplate defenderEffectTemplate)
        {
            this.defenderEffectTemplate = defenderEffectTemplate;
        }

        public abstract string UIText();
    }
}