namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public class CTimedDefenderEffect : CDefenderEffect, ITimedDefenderEffect
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

        public CTimedDefenderEffect(ITimedDefenderEffectTemplate timedDefenderEffectTemplate) : base(timedDefenderEffectTemplate)
        {
            this.timedDefenderEffectTemplate = timedDefenderEffectTemplate;
        }

        public override string UIText()
        {
            return timedDefenderEffectTemplate.NameInGame;
        }
    }
}