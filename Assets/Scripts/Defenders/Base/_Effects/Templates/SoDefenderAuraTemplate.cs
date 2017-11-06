using UnityEngine;

namespace GrimoireTD.Defenders.DefenderEffects
{
    [CreateAssetMenu(fileName = "NewDefenderAura", menuName = "Structures and Units/Defender Aura")]
    public class SoDefenderAuraTemplate : SoDefenderEffectTemplate, IDefenderAuraTemplate
    {
        [SerializeField]
        private int baseRange;

        [SerializeField]
        private bool affectsSelf;

        public int BaseRange
        {
            get
            {
                return baseRange;
            }
        }

        public bool AffectsSelf
        {
            get
            {
                return affectsSelf;
            }
        }

        public IDefenderAura GenerateDefenderAura(IDefender sourceDefender)
        {
            return new CDefenderAura(this, sourceDefender);
        }
    }
}