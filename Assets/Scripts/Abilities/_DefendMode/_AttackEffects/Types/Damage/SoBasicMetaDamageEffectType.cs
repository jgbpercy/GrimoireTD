using GrimoireTD.Dependencies;
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
                return DepsProv.TheAttackEffectTypeManager.GetWeakMetaDamageType(this);
            }
        }

        public IStrongMetaDamageEffectType StrongMetaDamageType
        {
            get
            {
                return DepsProv.TheAttackEffectTypeManager.GetStrongMetaDamageType(this);
            }
        }

        public override string EffectName()
        {
            return "Basic " + base.EffectName();
        }
    }
}