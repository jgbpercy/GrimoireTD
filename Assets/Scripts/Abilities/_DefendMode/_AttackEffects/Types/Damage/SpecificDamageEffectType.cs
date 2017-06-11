using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSpecificDamageEffectType",
    menuName = "Attack Effects/Damage Types/Specific Damage"
)]
public class SpecificDamageEffectType : DamageEffectType {

    [SerializeField]
    private SpecificDamageType specificDamageType;

    public SpecificDamageType SpecificDamageType
    {
        get
        {
            return specificDamageType;
        }
    }

    public MetaDamageType MetaDamageType
    {
        get
        {
            return DamageTypeManager.GetMetaDamageType(specificDamageType);
        }
    }
}
