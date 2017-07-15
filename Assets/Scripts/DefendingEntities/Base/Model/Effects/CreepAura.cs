
public class CreepAura : DefenderEffect
{

    private CreepAuraTemplate creepAuraTemplate;

    public CreepAuraTemplate CreepAuraTemplate
    {
        get
        {
            return creepAuraTemplate;
        }
    }

    public float Range
    {
        get
        {
            return creepAuraTemplate.BaseRange;
        }
    }

    public CreepAura(CreepAuraTemplate creepAuraTemplate) : base(creepAuraTemplate)
    {
        this.creepAuraTemplate = creepAuraTemplate;
    }
}
