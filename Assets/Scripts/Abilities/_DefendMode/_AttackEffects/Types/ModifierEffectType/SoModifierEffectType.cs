using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class SoModifierEffectType : SoAttackEffectType, IModifierEffectType
    {
        [SerializeField]
        private bool temporary;

        public bool Temporary
        {
            get
            {
                return temporary;
            }
        }
    }
}