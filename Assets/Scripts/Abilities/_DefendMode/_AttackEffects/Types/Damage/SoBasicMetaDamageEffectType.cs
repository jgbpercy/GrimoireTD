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

        public override float EffectOfArmor
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
                return GameModels.Models[0].AttackEffectTypeManager.GetWeakMetaDamageType(this);
            }
        }

        public IStrongMetaDamageEffectType StrongMetaDamageType
        {
            get
            {
                return GameModels.Models[0].AttackEffectTypeManager.GetStrongMetaDamageType(this);
            }
        }

        public override string EffectName()
        {
            return "Basic " + base.EffectName();
        }
    }
}