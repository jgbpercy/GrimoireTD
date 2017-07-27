public class TimedDefenderEffect : DefenderEffect
{
    private ITimedDefenderEffectTemplate timedDefenderEffectTemplate;

    public ITimedDefenderEffectTemplate TimedDefenderEffectTemplate
    {
        get
        {
            return timedDefenderEffectTemplate;
        }
    }

    public float Duration
    {
        get
        {
            return timedDefenderEffectTemplate.BaseDuration;
        }
    }

    public TimedDefenderEffect(ITimedDefenderEffectTemplate timedDefenderEffectTemplate) : base(timedDefenderEffectTemplate)
    {
        this.timedDefenderEffectTemplate = timedDefenderEffectTemplate;
    }

    public override string UIText()
    {
        return timedDefenderEffectTemplate.NameInGame;
    }
}
