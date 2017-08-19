using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewResistanceModifierEffectType",
        menuName = "Attack Effects/Resistance Modifier"
    )]
    public class SoResistanceModifierEffectType : SoModifierEffectType, IResistanceModifierEffectType
    {
        [SerializeField]
        private SoSpecificDamageEffectType resistanceToModify;

        public ISpecificDamageEffectType ResistanceToModify
        {
            get
            {
                return resistanceToModify;
            }
        }
    }
}