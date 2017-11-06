using UnityEngine;

namespace GrimoireTD.Defenders.DefenderEffects
{
    public class SoCreepAuraTemplate : SoDefenderEffectTemplate, ICreepAuraTemplate
    {
        [SerializeField]
        private float baseRange;

        public float BaseRange
        {
            get
            {
                return baseRange;
            }
        }
    }
}