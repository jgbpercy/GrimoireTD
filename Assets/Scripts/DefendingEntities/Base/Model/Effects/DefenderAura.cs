using System;

public class DefenderAura : DefenderEffect
{

    private DefenderAuraTemplate defenderAuraTemplate;

    private DefendingEntity sourceDefendingEntity;

    private Action<DefenderAura> OnClearAuraCallback;

    public DefenderAuraTemplate DefenderAuraTemplate
    {
        get
        {
            return defenderAuraTemplate;
        }
    }

    public DefendingEntity SourceDefendingEntity
    {
        get
        {
            return sourceDefendingEntity;
        }
    }

    public int Range
    {
        get
        {
            return defenderAuraTemplate.BaseRange;
        }
    }

    public DefenderAura(DefenderAuraTemplate defenderAuraTemplate, DefendingEntity sourceDefendingEntity) : base(defenderAuraTemplate)
    {
        this.defenderAuraTemplate = defenderAuraTemplate;
        this.sourceDefendingEntity = sourceDefendingEntity;
    }

    public void ClearAura()
    {
        OnClearAuraCallback(this);
    }

    public void RegisterForOnClearAuraCallback(Action<DefenderAura> callback)
    {
        OnClearAuraCallback += callback;
    }

    public void DeregisterForOnClearAuraCallback(Action<DefenderAura> callback)
    {
        OnClearAuraCallback -= callback;
    }
}
