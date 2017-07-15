
public abstract class DefenderEffect
{

    private DefenderEffectTemplate defenderEffectTemplate;

    public DefenderEffectTemplate DefenderEffectTemplate
    {
        get
        {
            return defenderEffectTemplate;
        }
    }

    public DefenderEffect(DefenderEffectTemplate defenderEffectTemplate)
    {
        this.defenderEffectTemplate = defenderEffectTemplate;
    }
}
