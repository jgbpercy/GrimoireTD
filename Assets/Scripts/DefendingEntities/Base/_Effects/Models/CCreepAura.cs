namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public class CCreepAura : CDefenderEffect, ICreepAura
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

        public CCreepAura(ICreepAuraTemplate creepAuraTemplate) : base(creepAuraTemplate)
        {
            this.creepAuraTemplate = creepAuraTemplate;
        }

        public override string UIText()
        {
            return creepAuraTemplate.NameInGame;
        }
    }
}