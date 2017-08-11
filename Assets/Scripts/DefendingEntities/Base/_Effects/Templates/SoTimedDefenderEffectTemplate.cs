using UnityEngine;

namespace GrimoireTD.DefendingEntities.DefenderEffects
{
    public class SoTimedDefenderEffectTemplate : SoDefenderEffectTemplate, IModeDurationDefenderEffectTemplate
    {
        [SerializeField]
        private float baseDuration;

        public float BaseDuration
        {
            get
            {
                return baseDuration;
            }
        }
    }
}