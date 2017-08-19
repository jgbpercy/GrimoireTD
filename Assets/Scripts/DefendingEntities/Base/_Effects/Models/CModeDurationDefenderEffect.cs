namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public class CModeDurationDefenderEffect : CDefenderEffect, IModeDurationDefenderEffect
    {
        private ModeDurationDefenderEffectTemplate modeDurationDefenderEffectTemplate;

        public ModeDurationDefenderEffectTemplate ModeDurationDefenderEffectTemplate
        {
            get
            {
                return modeDurationDefenderEffectTemplate;
            }
        }

        public int Duration
        {
            get
            {
                return modeDurationDefenderEffectTemplate.BaseDuration;
            }
        }

        public CModeDurationDefenderEffect(ModeDurationDefenderEffectTemplate modeDurationDefenderEffectTemplate) : base(modeDurationDefenderEffectTemplate)
        {
            this.modeDurationDefenderEffectTemplate = modeDurationDefenderEffectTemplate;
        }

        public override string UIText()
        {
            return modeDurationDefenderEffectTemplate.NameInGame;
        }
    }
}