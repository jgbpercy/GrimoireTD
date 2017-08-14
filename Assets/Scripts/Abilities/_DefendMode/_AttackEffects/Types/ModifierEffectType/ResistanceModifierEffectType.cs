using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewResistanceModifierEffectType",
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