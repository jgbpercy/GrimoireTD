using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class ModifierEffectType : AttackEffectType
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