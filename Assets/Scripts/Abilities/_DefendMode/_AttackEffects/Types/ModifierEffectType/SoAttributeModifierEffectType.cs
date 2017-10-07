using UnityEngine;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewAttributeChangeEffectType",
        menuName = "Attack Effects/Attribute Modifier"
    )]
    public class SoAttributeModifierEffectType : SoModifierEffectType, IAttributeModifierEffectType
    {
        [SerializeField]
        private CreepAttrName creepAttributeName;

        public CreepAttrName CreepAttributeName
        {
            get
            {
                return creepAttributeName;
            }
        }
    }
}