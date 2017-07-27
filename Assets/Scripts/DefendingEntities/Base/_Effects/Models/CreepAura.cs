public class CreepAura : DefenderEffect
{
    private ICreepAuraTemplate creepAuraTemplate;

    public ICreepAuraTemplate CreepAuraTemplate
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

    public CreepAura(ICreepAuraTemplate creepAuraTemplate) : base(creepAuraTemplate)
    {
        this.creepAuraTemplate = creepAuraTemplate;
    }

    public override string UIText()
    {
        return creepAuraTemplate.NameInGame;
    }
}
