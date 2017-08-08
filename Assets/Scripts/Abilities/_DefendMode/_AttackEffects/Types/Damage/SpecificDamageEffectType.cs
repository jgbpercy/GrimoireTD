using UnityEngine;

[CreateAssetMenu(
    fileName = "NewSpecificDamageEffectType",
    menuName = "Attack Effects/Damage Types/Specific Damage"
)]
public class SpecificDamageEffectType : DamageEffectType
{
    [SerializeField]
    private BasicMetaDamageEffectType basicMetaDamageEffectType;

    public BasicMetaDamageEffectType BasicMetaDamageEffectType
    {
        get
        {
            return basicMetaDamageEffectType;
        }
    }
}
