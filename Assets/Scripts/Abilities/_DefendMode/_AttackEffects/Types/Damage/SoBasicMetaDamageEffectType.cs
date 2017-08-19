using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewBasicMetaDamageEffectType",
        menuName = "Attack Effects/Damage Types/Basic Meta Damage"
    )]
    public class SoBasicMetaDamageEffectType : SoMetaDamageEffectType, IBasicMetaDamageEffectType
    {
        [SerializeField]
        private float effectOfArmor;

        public float EffectOfArmor
        {
            get
            {
                return effectOfArmor;
            }
        }

        public IWeakMetaDamageEffectType WeakMetaDamageType
        {
            get
            {
                return AttackEffectTypeManager.Instance.GetWeakMetaDamageType(this);
            }
        }

        public IStrongMetaDamageEffectType StrongMetaDamageType
        {
            get
            {
                return AttackEffectTypeManager.Instance.GetStrongMetaDamageType(this);
            }
        }

        public override string EffectName()
        {
            return "Basic " + base.EffectName();
        }
    }
}