namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public abstract class DefenderEffect
    {
        private IDefenderEffectTemplate defenderEffectTemplate;

        public IDefenderEffectTemplate DefenderEffectTemplate
        {
            get
            {
                return defenderEffectTemplate;
            }
        }

        public DefenderEffect(IDefenderEffectTemplate defenderEffectTemplate)
        {
            this.defenderEffectTemplate = defenderEffectTemplate;
        }

        public abstract string UIText();
    }
}