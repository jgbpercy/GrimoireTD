using System.Collections.Generic;
using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewWeakMetaDamageEffectType",
        menuName = "Attack Effects/Damage Types/Weak Meta Damage"
    )]
    public class WeakMetaDamageEffectType : MetaDamageEffectType
    {
        [SerializeField]
        private BasicMetaDamageEffectType basicMetaDamageType;

        public BasicMetaDamageEffectType BasicMetaDamageType
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
    }
}