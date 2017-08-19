using UnityEngine;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class SoAttackEffectType : ScriptableObject, IAttackEffectType
    {
        [SerializeField]
        private string effectName;

        public string ShortName
        {
            get
            {
                return effectName;
            }
        }

        public virtual string EffectName()
        {
            return effectName;
        }
    }
}