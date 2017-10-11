using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewWeakMetaDamageEffectType",
        menuName = "Attack Effects/Damage Types/Weak Meta Damage"
    )]
    public class SoWeakMetaDamageEffectType : SoMetaDamageEffectType, IWeakMetaDamageEffectType
    {
        [SerializeField]
        private SoBasicMetaDamageEffectType basicMetaDamageType;

        public IBasicMetaDamageEffectType BasicMetaDamageType
        {
            get
            {
                return basicMetaDamageType;
            }
        }

        public override string EffectName()
        {
            return "Weak " + base.EffectName();
        }

        public override float EffectOfArmor
        {
            get
            {
                return basicMetaDamageType.EffectOfArmor;
            }
        }
    }
}