namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public class CTimedDefenderEffect : CDefenderEffect, ITimedDefenderEffect
    {
        public ITimedDefenderEffectTemplate TimedDefenderEffectTemplate { get; }

        public float Duration
        {
            get
            {
                return TimedDefenderEffectTemplate.BaseDuration;
            }
        }

        public CTimedDefenderEffect(ITimedDefenderEffectTemplate timedDefenderEffectTemplate) : base(timedDefenderEffectTemplate)
        {
            this.TimedDefenderEffectTemplate = timedDefenderEffectTemplate;
        }

        public override string UIText()
        {
            return TimedDefenderEffectTemplate.NameInGame;
        }
    }
}