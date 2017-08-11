using System.Collections.Generic;

namespace GrimoireTD.Abilities.DefendMode.AttackEffects
{
    public class MetaDamageEffectType : DamageEffectType
    {
        public IReadOnlyList<SpecificDamageEffectType> SpecificDamageTypes
        {
            get
            {
                return AttackEffectTypeManager.Instance.GetSpecificDamageTypes(this);
            }
        }
    }
}