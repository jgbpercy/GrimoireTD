using UnityEngine;
using GrimoireTD.Attributes;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewAttributeChangeEffectType",
        menuName = "Attack Effects/Attribute Modifier"
    )]
    public class AttributeModifierEffectType : ModifierEffectType
    {
        [SerializeField]
        private CreepAttributeName creepAttributeName;

        public CreepAttributeName CreepAttributeName
        {
            get
            {
                return creepAttributeName;
            }
        }
    }
}