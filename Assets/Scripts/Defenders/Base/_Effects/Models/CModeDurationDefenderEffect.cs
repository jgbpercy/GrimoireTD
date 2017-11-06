namespace GrimoireTD.Defenders.DefenderEffects
{
    public class CModeDurationDefenderEffect : CDefenderEffect, IModeDurationDefenderEffect
    {
        public ModeDurationDefenderEffectTemplate ModeDurationDefenderEffectTemplate { get; }

        public int Duration
        {
            get
            {
                return ModeDurationDefenderEffectTemplate.BaseDuration;
            }
        }

        public CModeDurationDefenderEffect(ModeDurationDefenderEffectTemplate modeDurationDefenderEffectTemplate) : base(modeDurationDefenderEffectTemplate)
        {
            ModeDurationDefenderEffectTemplate = modeDurationDefenderEffectTemplate;
        }

        public override string UIText()
        {
            return ModeDurationDefenderEffectTemplate.NameInGame;
        }
    }
}