
public class TimedDefenderEffect : DefenderEffect
{

    private TimedDefenderEffectTemplate timedDefenderEffectTemplate;

    public TimedDefenderEffectTemplate TimedDefenderEffectTemplate
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

    public TimedDefenderEffect(TimedDefenderEffectTemplate timedDefenderEffectTemplate) : base(timedDefenderEffectTemplate)
    {
        this.timedDefenderEffectTemplate = timedDefenderEffectTemplate;
    }
}
