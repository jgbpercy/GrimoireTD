using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    [CreateAssetMenu(
        fileName = "NewBlockModifierEffectType",
        menuName = "Attack Effects/Block Modifier"
    )]
    public class SoBlockModifierEffectType : SoModifierEffectType, IBlockModifierEffectType
    {
        [SerializeField]
        private SoSpecificDamageEffectType blockTypeToModify;

        public ISpecificDamageEffectType BlockTypeToModify
        {
            get
            {
                return blockTypeToModify;
            }
        }
    }
}