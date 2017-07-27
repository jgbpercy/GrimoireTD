public class ModeDurationDefenderEffect : DefenderEffect
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

    public ModeDurationDefenderEffect(ModeDurationDefenderEffectTemplate modeDurationDefenderEffectTemplate) : base(modeDurationDefenderEffectTemplate)
    {
        this.modeDurationDefenderEffectTemplate = modeDurationDefenderEffectTemplate;
    }

    public override string UIText()
    {
        return modeDurationDefenderEffectTemplate.NameInGame;
    }
}
