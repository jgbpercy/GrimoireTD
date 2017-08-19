using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewStrongMetaDamageEffectType",
        menuName = "Attack Effects/Damage Types/Strong Meta Damage"
    )]
    public class SoStrongMetaDamageEffectType : SoMetaDamageEffectType, IStrongMetaDamageEffectType
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
            return "Strong " + base.EffectName();
        }
    }
}