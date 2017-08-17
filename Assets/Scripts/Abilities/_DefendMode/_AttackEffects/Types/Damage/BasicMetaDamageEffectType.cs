using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewBasicMetaDamageEffectType",
        menuName = "Attack Effects/Damage Types/Basic Meta Damage"
    )]
    public class BasicMetaDamageEffectType : MetaDamageEffectType
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

        public WeakMetaDamageEffectType WeakMetaDamageType
        {
            get
            {
                return AttackEffectTypeManager.Instance.GetWeakMetaDamageType(this);
            }
        }

        public StrongMetaDamageEffectType StrongMetaDamageType
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