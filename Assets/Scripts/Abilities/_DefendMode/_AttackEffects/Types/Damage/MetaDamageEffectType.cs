using System.Collections.Generic;

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
