using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewBlockModifierEffectType",
        menuName = "Attack Effects/Block Modifier"
    )]
    public class BlockModifierEffectType : ModifierEffectType
    {
        [SerializeField]
        private SpecificDamageEffectType blockTypeToModify;

        public SpecificDamageEffectType BlockTypeToModify
        {
            get
            {
                return blockTypeToModify;
            }
        }
    }
}