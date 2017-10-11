using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewSpecificDamageEffectType",
        menuName = "Attack Effects/Damage Types/Specific Damage"
    )]
    public class SoSpecificDamageEffectType : SoDamageEffectType, ISpecificDamageEffectType
    {
        [SerializeField]
        private SoBasicMetaDamageEffectType basicMetaDamageEffectType;

        public IBasicMetaDamageEffectType BasicMetaDamageEffectType
        {
            get
            {
                return basicMetaDamageEffectType;
            }
        }

        public override float EffectOfArmor
        {
            get
            {
                return basicMetaDamageEffectType.EffectOfArmor;
            }
        }
    }
}