using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MetaDamageEffectType : DamageEffectType {

    [SerializeField]
    private MetaDamageType metaDamageType;

    public MetaDamageType MetaDamageType
    {
        get
        {
            return MetaDamageType;
        }
    }

    public List<SpecificDamageType> SpecificDamageTypes
    {
        get
        {
            return DamageTypeManager.GetSpecificDamageTypes(metaDamageType);
        }
    }
}
