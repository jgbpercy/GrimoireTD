namespace GrimoireTD.Defenders.DefenderEffects
{
    public class CCreepAura : CDefenderEffect, ICreepAura
    {
        public ICreepAuraTemplate CreepAuraTemplate { get; }

        public float Range
        {
            get
            {
                return CreepAuraTemplate.BaseRange;
            }
        }

        public CCreepAura(ICreepAuraTemplate creepAuraTemplate) : base(creepAuraTemplate)
        {
            CreepAuraTemplate = creepAuraTemplate;
        }

        public override string UIText()
        {
            return CreepAuraTemplate.NameInGame;
        }
    }
}