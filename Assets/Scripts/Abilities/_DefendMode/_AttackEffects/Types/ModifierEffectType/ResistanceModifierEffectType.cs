using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewAttributeChangeEffectType",
        menuName = "Attack Effects/Resistance Modifier"
    )]
    public class ResistanceModifierEffectType : ModifierEffectType
    {
        [SerializeField]
        private SpecificDamageEffectType resistanceToModify;

        public SpecificDamageEffectType ResistanceToModify
        {
            get
            {
                return resistanceToModify;
            }
        }
    }
}